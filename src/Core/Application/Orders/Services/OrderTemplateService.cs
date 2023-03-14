using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Orders.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Products;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Application.Exceptions;
using Mapster;
using MyReliableSite.Domain.Common;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Shared.DTOs.Categories;
using MyReliableSite.Shared.DTOs.Departments;

namespace MyReliableSite.Application.Orders.Services;
public class OrderTemplateService : IOrderTemplateService
{
    private readonly IStringLocalizer<OrderTemplateService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly IFileStorageService _file;

    public OrderTemplateService()
    {
    }

    public OrderTemplateService(IRepositoryAsync repository, IFileStorageService file, IStringLocalizer<OrderTemplateService> localizer, ICurrentUser user)
    {
        _repository = repository;
        _localizer = localizer;
        _user = user;
        _file = file;
    }

    #region new

    public async Task<Result<Guid>> CreateOrderTemplateAsync(CreateOrderTemplateRequest request)
    {
        // Check Already exists
        bool productExists = await _repository.ExistsAsync<OrderTemplate>(a => a.Name == request.Name && a.DeletedOn == null);
        if (productExists) throw new EntityAlreadyExistsException(string.Format(_localizer["OrderTemplate.alreadyexists"], request.Name));

        string productThumbnailPath = null;
        if (request.Thumbnail != null) productThumbnailPath = await _file.UploadAsync<Product>(request.Thumbnail, FileType.Image);

        string userId = _user.GetUserId().ToString();

        // Add New Product
        var orderTemplate = new OrderTemplate(request.ProductName, request.Description, request.IsActive, request.Name, request.Description, productThumbnailPath, (PaymentType)request.PaymentType, userId, request.BillingCycle, request.Notes);

        // Add OrderTemplate Line Items
        if (request.OrderTemplateLineItems != null)
        {
            foreach (var lineItem in request.OrderTemplateLineItems)
                orderTemplate.OrderTemplateLineItems.Add(new OrderTemplateLineItem(lineItem.LineItem, lineItem.Price, lineItem.PriceType));
        }

        orderTemplate.DomainEvents.Add(new StatsChangedEvent());

        var orderTemplateId = await _repository.CreateAsync<OrderTemplate>(orderTemplate);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(orderTemplateId);
    }

    public async Task<Result<Guid>> UpdateOrderTemplateAsync(UpdateOrderTemplateRequest request, Guid id)
    {
        var spec = new BaseSpecification<OrderTemplate>();
        spec.Includes.Add(a => a.OrderTemplateLineItems);

        var orderTemplate = await _repository.GetByIdAsync<OrderTemplate>(id, spec);
        if (orderTemplate == null) throw new EntityNotFoundException(string.Format(_localizer["orderTemplate.notfound"], id));

        string orderTemplateThumbnail = null;
        if (request.Thumbnail != null) orderTemplateThumbnail = await _file.UploadAsync<OrderTemplate>(request.Thumbnail, FileType.Image);

        string userId = _user.GetUserId().ToString();

        var updatedOrderTemplate = orderTemplate.Update(request.ProductName, request.Description, request.IsActive, request.Name, request.Description, orderTemplateThumbnail, (PaymentType)request.PaymentType, userId, request.BillingCycle, request.Notes);

        // Update OrderTemplate Line Items
        foreach (var lineItem in request.OrderTemplateLineItems)
        {
            var currentLineItem = updatedOrderTemplate.OrderTemplateLineItems.FirstOrDefault(x => x.Id == lineItem.Id);

            // New OrderTemplate Line Item
            if (currentLineItem == null)
            {
                await _repository.CreateAsync(new OrderTemplateLineItem(lineItem.LineItem, lineItem.Price, id, lineItem.PriceType));
            } // Update/Remove OrderTemplate Line Item
            else
            {
                // Delete OrderTemplate Line Item
                if (lineItem.IsDeleted)
                    await _repository.RemoveByIdAsync<OrderTemplateLineItem>(currentLineItem.Id);
                else
                    currentLineItem = currentLineItem.Update(lineItem.LineItem, lineItem.Price, lineItem.PriceType);
            }
        }

        await _repository.UpdateAsync<OrderTemplate>(updatedOrderTemplate);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }
    #endregion

    public async Task<Result<OrderTemplateDetailsDto>> GetOrderTemplateDetailsAsync(Guid id)
    {
        var spec = new BaseSpecification<OrderTemplate>();
        spec.Includes.Add(a => a.OrderTemplateLineItems);

        var orderTemplate = await _repository.GetByIdAsync<OrderTemplate, OrderTemplateDetailsDto>(id, spec);
        if (orderTemplate == null) throw new EntityNotFoundException(string.Format(_localizer["ordertemplate.notfound"], id));
        if (!string.IsNullOrEmpty(orderTemplate.Thumbnail))
        {
            orderTemplate.Base64Image = await _file.ReturnBase64StringOfImageFileAsync(orderTemplate.Thumbnail);
        }

        if (orderTemplate.OrderTemplateLineItems != null)
        {
            orderTemplate.OrderTemplateLineItems = orderTemplate.OrderTemplateLineItems.OrderBy(m => m.CreatedOn).ToList();
        }

        return await Result<OrderTemplateDetailsDto>.SuccessAsync(orderTemplate);
    }

    public async Task<PaginatedResult<OrderTemplateDto>> SearchAsync(OrderTemplateListFilter filter)
    {
        var spec = new BaseSpecification<OrderTemplate>();
        spec.Includes.Add(a => a.OrderTemplateLineItems);

        var orderTemplates = await _repository.GetSearchResultsAsync<OrderTemplate, OrderTemplateDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, null, spec);
        var lineItems = await _repository.GetListAsync<OrderTemplateLineItem>(x => orderTemplates.Data.Select(x => x.Id).Distinct().Contains(x.OrderTemplateId));

        foreach (var orderTemplate in orderTemplates.Data)
        {
            var thisLineItems = lineItems.Where(x => x.OrderTemplateId == orderTemplate.Id);
            if (thisLineItems != null)
            {
                orderTemplate.OrderTemplateLineItems = thisLineItems.Select(x => new OrderTemplateLineItemDto() { Id = x.Id, LineItem = x.LineItem, Price = x.Price, PriceType = x.PriceType, CreatedOn = x.CreatedOn }).OrderBy(x => x.CreatedOn).ToList();
                orderTemplate.TotalPriceOfLineItems = orderTemplate.OrderTemplateLineItems.Sum(x => x.Price);
            }

            if (!string.IsNullOrEmpty(orderTemplate.Thumbnail))
            {
                orderTemplate.Base64Image = await _file.ReturnBase64StringOfImageFileAsync(orderTemplate.Thumbnail);
            }

        }

        return orderTemplates;
    }

    public async Task<List<OrderTemplateDto>> GetAllAsync(string tenant)
    {
        var templates = await _repository.GetListAsync<OrderTemplate>(m => m.Tenant == tenant);
        return templates.Adapt<List<OrderTemplateDto>>();

    }

    /*
    public async Task<Result<Guid>> CreateOrderTemplateAsync(CreateOrderTemplateRequest request)
    {
        string userId = _user.GetUserId().ToString();

        // New Product
        OrderTemplate orderTemplate = new OrderTemplate(request.TemplateName, request.Notes, 0);

        decimal total = 0;
        foreach (var productId in request.ProductIds)
        {
            var specs = new BaseSpecification<Product>();
            specs.Includes.Add(x => x.ProductLineItems);

            // Get Product Detail with its product lines item details
            var product = await _repository.GetByIdAsync<Product>(productId, specs);

            decimal subTotal = 0;

            // Add Line Items for Audit
            foreach (var item in product.ProductLineItems)
            {
                orderTemplate.OrderProductLineItems.Add(new OrderProductLineItem(item.LineItem, item.Price, product.Id, item.Id));
                subTotal += item.Price;
            }

            // Store SubTotal without VAT
            total += subTotal;
        }

        orderTemplate.Total = total;

        var orderId = await _repository.CreateAsync<OrderTemplate>(orderTemplate);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(orderId);
    }

    public async Task<Result<Guid>> UpdateOrderTemplateAsync(UpdateOrderTemplateRequest request, Guid id)
    {
        var orderTemplate = await _repository.GetByIdAsync<OrderTemplate>(id);
        if (orderTemplate == null) throw new EntityNotFoundException(string.Format(_localizer["orderTemplate.notfound"], id));

        var updatedOrder = orderTemplate.Update(request.TemplateName, request.Notes);

        updatedOrder.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync<OrderTemplate>(updatedOrder);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }
    */
    public async Task<Result<Guid>> DeleteOrderTemplateAsync(Guid id)
    {
        var orderTemplate = await _repository.GetByIdAsync<OrderTemplate>(id);
        if (orderTemplate == null) throw new EntityNotFoundException(string.Format(_localizer["orderTemplate.notfound"], id));

        orderTemplate.DeletedOn = DateTime.UtcNow;
        orderTemplate.DeletedBy = _user.GetUserId();

        orderTemplate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }
}
