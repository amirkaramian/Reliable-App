using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Dashboard;
using Mapster;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Application.Storage;
using MyReliableSite.Domain.Common;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Domain.Products.Events;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Settings;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Domain.Constants;
using Microsoft.Extensions.Options;
using MyReliableSite.Domain.Departments;
using MediatR;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Reports;
using MyReliableSite.Shared.DTOs.Scripting;

using MyReliableSite.Shared.DTOs.ManageModule;
using Newtonsoft.Json;
using MyReliableSite.Application.ManageModule.Interfaces;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Application.Products.Services;
public class ProductService : IProductService
{
    private readonly IFileStorageService _file;
    private readonly IStringLocalizer<ProductService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly IJobService _jobService;
    private readonly MailSettings _mailSettings;
    private readonly IEmailTemplateService _templateService;
    private readonly IMailService _mailService;
    private readonly INotificationService _notificationService;
    private readonly IUserModuleManagementService _service;
    private readonly IUserService _userService;

    public ProductService(IRepositoryAsync repository, IUserService userService, INotificationService notificationService, IMailService mailService, IEmailTemplateService templateService, IStringLocalizer<ProductService> localizer, IFileStorageService file, ICurrentUser user, IJobService jobService, IOptions<MailSettings> mailSettings, IUserModuleManagementService service)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _user = user;
        _jobService = jobService;
        _mailSettings = mailSettings.Value;
        _templateService = templateService;
        _mailService = mailService;
        _notificationService = notificationService;
        _userService = userService;
        _service = service;
    }

    public async Task<Result<ProductDetailDto>> GetProductDetailsAsync(Guid id)
    {
        var spec = new BaseSpecification<Product>();
        spec.Includes.Add(a => a.ProductDepartments);
        spec.Includes.Add(a => a.ProductLineItems);

        var product = await _repository.GetByIdAsync<Product, ProductDetailDto>(id, spec);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        var bill = await _repository.FirstByConditionAsync<Bill>(m => m.OrderId == product.OrderId);

        if (bill != null)
        {
            product.InvoiceId = bill.Id;
        }

        if (!string.IsNullOrEmpty(product.Thumbnail))
        {
            product.Base64Image = product.Thumbnail; // await _file.ReturnBase64StringOfImageFileAsync(product.Thumbnail);
        }

        if (!string.IsNullOrEmpty(product.AssignedToClientId))
        {
            var user = await _userService.GetAsync(product.AssignedToClientId);
            if (user?.Data != null)
            {
                product.AssignedClient = user.Data.FullName;
            }
        }

        if (product.ProductLineItems != null)
        {
            product.ProductLineItems = product.ProductLineItems.OrderByDescending(m => m.CreatedOn).ToList();
        }

        return await Result<ProductDetailDto>.SuccessAsync(product);
    }

    public async Task<PaginatedResult<ProductDto>> SearchAsync(ProductListFilter filter)
    {
        var products = await _repository.GetSearchResultsAsync<Product, ProductDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);

        var lineItems = await _repository.GetListAsync<ProductLineItems>(x => products.Data.Select(x => x.Id).Distinct().Contains(x.ProductId));

        foreach (var product in products.Data)
        {
            product.ProductLineItems = lineItems.Where(x => x.ProductId == product.Id).Select(x => new ProductLineItemDto() { Id = x.Id, LineItem = x.LineItem, Price = x.Price, PriceType = x.PriceType }).ToList();
            product.TotalPriceOfLineItems = product.ProductLineItems.Sum(x => x.Price);
            if (!string.IsNullOrEmpty(product.Thumbnail))
            {
                product.Base64Image = product.Thumbnail; // await _file.ReturnBase64StringOfImageFileAsync(product.Thumbnail);
            }

            if (!string.IsNullOrEmpty(product.AssignedToClientId))
            {
                var user = await _userService.GetAsync(product.AssignedToClientId);
                if (user != null && user.Data != null)
                {
                    product.AssignedClient = user.Data.FullName;
                }
            }

        }

        return products;
    }

    public async Task<PaginatedResult<ProductDto>> SearchAsClientAsync(ProductListFilter filter)
    {
        var products = await _repository.GetSearchResultsAsync<Product, ProductDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.AssignedToClientId == _user.GetUserId().ToString());

        var lineItems = await _repository.GetListAsync<ProductLineItems>(x => products.Data.Select(x => x.Id).Distinct().Contains(x.ProductId));

        foreach (var product in products.Data)
        {
            product.ProductLineItems = lineItems.Where(x => x.ProductId == product.Id).Select(x => new ProductLineItemDto() { Id = x.Id, LineItem = x.LineItem, Price = x.Price, PriceType = x.PriceType }).ToList();
            product.TotalPriceOfLineItems = product.ProductLineItems.Sum(x => x.Price);
            if (!string.IsNullOrEmpty(product.Thumbnail))
            {
                product.Base64Image = product.Thumbnail; // await _file.ReturnBase64StringOfImageFileAsync(product.Thumbnail);
            }

            if (!string.IsNullOrEmpty(product.AssignedToClientId))
            {
                var user = await _userService.GetAsync(product.AssignedToClientId);
                if (user?.Data != null)
                {
                    product.AssignedClient = user.Data.FullName;
                }
            }

        }

        return products;
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        // Check Already exists
        // bool productExists = await _repository.ExistsAsync<Product>(a => a.Name == request.Name && a.AssignedToClientId == request.AssignedToClientId && a.DeletedOn == null);
        // if (productExists) throw new EntityAlreadyExistsException(string.Format(_localizer["product.alreadyexists"], request.Name));

        string productThumbnailPath = null;
        if (request.Thumbnail != null && !string.IsNullOrEmpty(request.Thumbnail.Name)) productThumbnailPath = _file.GetBase64Image(request.Thumbnail);

        string userId = _user.GetUserId().ToString();

        // Add New Product
        var product = new Product(
            request.Name,
            request.Description,
            productThumbnailPath,
            (ProductStatus)request.Status,
            (PaymentType)request.PaymentType,
            request.Notes,
            request.RegistrationDate,
            request.NextDueDate,
            request.TerminationDate,
            request.OverrideSuspensionDate,
            request.OverrideTerminationDate,
            request.AssignedToClientId,
            request.BillingCycle,
            request.DedicatedIP,
            request.AssginedIPs);

        if (!string.IsNullOrEmpty(request.AssignedToClientId))
        {
            var user = await _userService.GetAsync(request.AssignedToClientId);

            // Add Product Departments
            if (user.Data?.DepartmentIds != null)
            {
                foreach (var department in user.Data.DepartmentIds)
                    product.ProductDepartments.Add(new ProductDepartments(department));
            }
        }

        // Add Product Line Items
        if (request.ProductLineItems != null)
        {
            foreach (var lineItem in request.ProductLineItems)
                product.ProductLineItems.Add(new ProductLineItems(lineItem.LineItem, lineItem.Price, lineItem.PriceType));
        }

        product.DomainEvents.Add(new ProductCreatedEvent(product, request.ExtraData));
        product.DomainEvents.Add(new StatsChangedEvent());

        var productId = await _repository.CreateAsync<Product>(product);

        return product;

    }

    public async Task<Product> CreateProductWHMCSAsync(CreateProductRequestWHMCS request)
    {
        // Check Already exists
        // bool productExists = await _repository.ExistsAsync<Product>(a => a.Name == request.Name && a.AssignedToClientId == request.AssignedToClientId && a.DeletedOn == null);
        // if (productExists) throw new EntityAlreadyExistsException(string.Format(_localizer["product.alreadyexists"], request.Name));

        string productThumbnailPath = null;
        if (request.Thumbnail != null) productThumbnailPath = _file.GetBase64Image(request.Thumbnail);

        string userId = _user.GetUserId().ToString();

        // Add New Product
        var product = new Product(
            request.Name,
            request.Description,
            productThumbnailPath,
            (ProductStatus)request.Status,
            (PaymentType)request.PaymentType,
            request.Notes,
            request.RegistrationDate,
            request.NextDueDate,
            request.TerminationDate,
            request.OverrideSuspensionDate,
            request.OverrideTerminationDate,
            request.AssignedToClientId,
            request.BillingCycle,
            request.DedicatedIP,
            request.AssginedIPs);

        product.OldOrderId = request.OldOrderId;
        product.OldProductId = request.OldProductId;
        product.ServerId = request.ServerId;
        product.DomainName = request.DomainName;

        // Add Product Departments
        // if (request.ProductDepartments != null)
        // {
        //    foreach (var departmentId in request.ProductDepartments)
        //        product.ProductDepartments.Add(new ProductDepartments(departmentId));
        // }

        // Add Product Line Items
        if (request.ProductLineItems != null)
        {
            foreach (var lineItem in request.ProductLineItems)
                product.ProductLineItems.Add(new ProductLineItems(lineItem.LineItem, lineItem.Price, lineItem.PriceType));
        }

        product.DomainEvents.Add(new ProductCreatedEvent(product, request.ExtraData));
        product.DomainEvents.Add(new StatsChangedEvent());

        var productId = await _repository.CreateAsync<Product>(product);

        var userAdmin = await _userService.GetAsync(request.AdminAssigned);
        string createProductUrl = $"https://admin.myreliablesite.m2mbeta.com/admin/dashboard/billing/products-services/list/details/{product.Id}";
        await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { userAdmin.Data }, Shared.DTOs.EmailTemplates.EmailTemplateType.ProductCreated, $"Product Name : {product.Name}", null, createProductUrl);

        if (string.IsNullOrWhiteSpace(request.AssignedToClientId))
        {
            var userClient = await _userService.GetAsync(request.AssignedToClientId);
            await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { userClient.Data }, Shared.DTOs.EmailTemplates.EmailTemplateType.ProductAssignment, $"Order Name : {product.Name}", null, createProductUrl);
        }

        return product;

    }

    public async Task<Result<Guid>> CreateProductAsync(CreateProductRequest request, bool isFromOrder = false)
    {
        // Check Already exists
        // bool productExists = await _repository.ExistsAsync<Product>(a => a.Name == request.Name && a.AssignedToClientId == request.AssignedToClientId && a.DeletedOn == null);
        // if (productExists) throw new EntityAlreadyExistsException(string.Format(_localizer["product.alreadyexists"], request.Name));

        string productThumbnailPath = null;
        if (request.Thumbnail != null) productThumbnailPath = _file.GetBase64Image(request.Thumbnail);

        string userId = _user.GetUserId().ToString();

        // Add New Product
        var product = new Product(
            request.Name,
            request.Description,
            productThumbnailPath,
            (ProductStatus)request.Status,
            (PaymentType)request.PaymentType,
            request.Notes,
            request.RegistrationDate,
            request.NextDueDate,
            request.TerminationDate,
            request.OverrideSuspensionDate,
            request.OverrideTerminationDate,
            request.AssignedToClientId,
            request.BillingCycle,
            request.DedicatedIP,
            request.AssginedIPs);

        if (!string.IsNullOrEmpty(request.AssignedToClientId))
        {
            var user = await _userService.GetAsync(request.AssignedToClientId);

            // Add Product Departments
            if (user.Data?.DepartmentIds != null)
            {
                foreach (var department in user.Data.DepartmentIds)
                    product.ProductDepartments.Add(new ProductDepartments(department));
            }
        }

        // Add Product Line Items
        if (request.ProductLineItems != null)
        {
            foreach (var lineItem in request.ProductLineItems)
                product.ProductLineItems.Add(new ProductLineItems(lineItem.LineItem, lineItem.Price, lineItem.PriceType));
        }

        product.DomainEvents.Add(new ProductCreatedEvent(product, request.ExtraData));
        product.DomainEvents.Add(new StatsChangedEvent());

        var productId = await _repository.CreateAsync<Product>(product);

        if (!isFromOrder)
        {
            await _repository.SaveChangesAsync();
        }

        return await Result<Guid>.SuccessAsync(productId);
    }

    public async Task<Result<Guid>> UpdateProductAsync(UpdateProductRequest request, Guid id)
    {
        var spec = new BaseSpecification<Product>();
        spec.Includes.Add(a => a.ProductDepartments);
        spec.Includes.Add(a => a.ProductLineItems);

        var product = await _repository.GetByIdAsync<Product>(id, spec);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        string productThumbnail = null;
        if (request.Thumbnail != null) productThumbnail = _file.GetBase64Image(request.Thumbnail);

        string userId = _user.GetUserId().ToString();

        var updatedProduct = product.Update(request.Name, request.Description, productThumbnail, (ProductStatus)request.Status, (PaymentType)request.PaymentType, request.Notes, request.RegistrationDate, request.NextDueDate, request.TerminationDate, request.OverrideSuspensionDate, request.OverrideTerminationDate, request.AssignedToClientId, request.BillingCycle, request.DedicatedIP, request.AssginedIPs);

        // Update Product Departments

        List<Guid> listedDepartments = new List<Guid>();
        if (!string.IsNullOrEmpty(request.AssignedToClientId))
        {
            var user = await _userService.GetAsync(request.AssignedToClientId);

            // Add Product Departments
            if (user.Data?.DepartmentIds != null)
            {
                foreach (var productDepartment in user.Data.DepartmentIds)
                {
                    var department = updatedProduct.ProductDepartments.FirstOrDefault(x => x.DepartmentId == productDepartment);

                    // if null its new
                    // if not null mean dont delete it just skip it
                    if (department == null)
                    {
                        listedDepartments.Add(productDepartment);
                        await _repository.CreateAsync(new ProductDepartments(productDepartment, id));
                    }
                    else if (department != null)
                    {
                        listedDepartments.Add(productDepartment);
                    }
                }
            }
        }

        var departmentsToRemove = updatedProduct.ProductDepartments.Where(x => !listedDepartments.Contains(x.DepartmentId)).ToList();

        if (departmentsToRemove != null && departmentsToRemove.Any())
        {
            foreach (var item in departmentsToRemove)
            {
                updatedProduct.ProductDepartments.Remove(item);
            }
        }

        // Update Product Line Items
        foreach (var lineItem in request.ProductLineItems)
        {
            var currentLineItem = updatedProduct.ProductLineItems.FirstOrDefault(x => x.Id == lineItem.Id);

            // New Product Line Item
            if (currentLineItem == null)
            {
                await _repository.CreateAsync(new ProductLineItems(lineItem.LineItem, lineItem.Price, id, lineItem.PriceType));
            } // Update/Remove Product Line Item
            else
            {
                // Delete Product Line Item
                if (lineItem.IsDeleted)
                    await _repository.RemoveByIdAsync<ProductLineItems>(currentLineItem.Id);
                else
                    currentLineItem = currentLineItem.Update(lineItem.LineItem, lineItem.Price, lineItem.PriceType);
            }
        }

        updatedProduct.DomainEvents.Add(new ProductUpdatedEvent(updatedProduct));

        await _repository.UpdateAsync<Product>(updatedProduct);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> AssignProductToClientAsync(Guid id, string clientId)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        if (product.AssignedToClientId != null) throw new EntityAlreadyExistsException(string.Format(_localizer["product.alreadyassigned"], id));

        var updatedProduct = product.Update(clientId);

        if (!string.IsNullOrEmpty(clientId))
        {
            var user = await _userService.GetAsync(clientId);

            // Add Product Departments
            if (user.Data?.DepartmentIds != null)
            {
                foreach (var department in user.Data.DepartmentIds)
                    updatedProduct.ProductDepartments.Add(new ProductDepartments(department));
            }
        }

        updatedProduct.DomainEvents.Add(new ProductUpdatedEvent(updatedProduct));

        await _repository.UpdateAsync<Product>(updatedProduct);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UnassignProductToClientAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        var updatedProduct = product.Update(null);
        updatedProduct.ProductDepartments.Clear();
        updatedProduct.DomainEvents.Add(new ProductUpdatedEvent(updatedProduct));

        await _repository.UpdateAsync<Product>(updatedProduct);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<List<ProductDto>>> AvailableProductsForClients()
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.AssignedToClientId == null);

        return await Result<List<ProductDto>>.SuccessAsync(products.Adapt<List<ProductDto>>());
    }

    public async Task<Result<List<ProductDto>>> GetProductsListAsync()
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.DeletedOn == null);

        return await Result<List<ProductDto>>.SuccessAsync(products.Adapt<List<ProductDto>>());
    }

    public async Task<Result<List<ProductEXL>>> GetProductListAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var products = await _repository.QueryWithDtoAsync<ProductEXL>($@"SELECT P.*
                                                                                                        FROM Products P
                                                                                                        WHERE ((CONVERT(date, [P].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [P].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and AssignedToClientId = '{userId}' ORDER BY P.CreatedOn ASC");
        return await Result<List<ProductEXL>>.SuccessAsync(products.ToList());
    }

    public async Task<PaginatedResult<ProductDto>> GetProductsWithStatus(string filter, ProductListFilter listFilter)
    {
        bool correct = Enum.TryParse(filter, true, out ProductStatus status);
        if (!correct)
        {
            var empty = new List<Product>();
            var emptyResult = PaginatedResult<ProductDto>.Success(empty.Adapt<List<ProductDto>>(), 0, 0, 0);
            emptyResult.TotalPages = 0;
            return emptyResult;
        }

        int pagenumber = listFilter.PageNumber;
        int pagesize = listFilter.PageSize;
        var products = await _repository
            .GetSearchResultsAsync<Product, ProductDto>(pageNumber: pagenumber, pageSize: pagesize, keyword: listFilter.Keyword, advancedSearch: listFilter.AdvancedSearch, expression: x => x.Status == status && x.AssignedToClientId == _user.GetUserId().ToString());
        return products;
    }

    public async Task<Result<Guid>> ChangeProductModule(Guid id, ProductModuleRequest request)
    {
        Product product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));
        product.ModuleName = request.Module;
        product.ProductSetup = request.ProductSetup;
        product.ExtraData = request.ExtraData.ToString();
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    private async Task<Result<int>> GetActiveProductsCountAsync(string userId)
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.Status == ProductStatus.Active && x.AssignedToClientId == userId);
        return await Result<int>.SuccessAsync(products.Count());
    }

    private async Task<Result<int>> GetSupendedProductsCountAsync(string userId)
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.Status == ProductStatus.Suspended && x.AssignedToClientId == userId);
        return await Result<int>.SuccessAsync(products.Count());
    }

    private async Task<Result<int>> GetPendingProductsCountAsync(string userId)
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.Status == ProductStatus.Pending && x.AssignedToClientId == userId);
        return await Result<int>.SuccessAsync(products.Count());
    }

    private async Task<Result<int>> GetCancelledProductsAsync(string userId)
    {
        var products = await _repository.FindByConditionAsync<Product>(x => x.Status == ProductStatus.Cancelled && x.AssignedToClientId == userId);
        return await Result<int>.SuccessAsync(products.Count());
    }

    public async Task<Result<ProductCounts>> GetProductCounts()
    {
        string userId = _user.GetUserId().ToString();
        var user = await _userService.GetUserProfileAsync(userId);
        if (user?.Data != null && !string.IsNullOrEmpty(user.Data.ParentID))
            userId = user.Data.ParentID;
        var active = await GetActiveProductsCountAsync(userId);
        var suspended = await GetSupendedProductsCountAsync(userId);
        var pending = await GetPendingProductsCountAsync(userId);
        var cancelled = await GetCancelledProductsAsync(userId);

        ProductCounts productCounts = new(active.Data, suspended.Data, pending.Data, cancelled.Data);
        return await Result<ProductCounts>.SuccessAsync(productCounts);
    }

    public async Task<Result<Guid>> DeleteProductAsync(Guid id)
    {
        var productToDelete = await _repository.GetByIdAsync<Product>(id);
        if (productToDelete == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        productToDelete.DeletedOn = DateTime.UtcNow;
        productToDelete.DeletedBy = _user.GetUserId();

        productToDelete.DomainEvents.Add(new ProductDeletedEvent(productToDelete));

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    #region automation modules

    public async Task<Result<Guid>> SuspensionOfProductAsync(Guid id, string suspendedReason)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));
        product.SuspendedReason = suspendedReason;
        product.OverrideSuspensionDate = DateTime.UtcNow;
        product.Status = ProductStatus.Suspended;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync(product);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> PendingProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.Status = ProductStatus.Pending;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync<Product>(product);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UnSuspensionOfProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.OverrideSuspensionDate = null;
        product.Status = ProductStatus.Active;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync<Product>(product);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> ActivateProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.OverrideSuspensionDate = null;
        product.Status = ProductStatus.Active;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync<Product>(product);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> TerminationOfProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.TerminationDate = DateTime.UtcNow;
        product.Status = ProductStatus.Cancelled;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync(product);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> CancellationRequestOfProductAsync(Guid id, bool sendEmail = true)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.Status = ProductStatus.Cancelled;

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync(product);

        await _repository.SaveChangesAsync();
        if (sendEmail)
            await SendEmailForProductStatusChangeAsync(product);
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> SendCancelRequestEmail(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));
        var user = await _userService.GetAsync(product.AssignedToClientId);
        if (user?.Data == null) return await Result<Guid>.SuccessAsync(id);
        string token = await _userService.GenerateUserToken();
        var template = await _templateService.GenerateEmailBodyForProductCancellation(product, user.Data, token);
        var mailRequest = new MailRequest
        {
            From = _mailSettings.From,
            To = new List<string> { user.Data.Email },
            Body = template.Body,
            Subject = template.Subject
        };
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> ConfirmCancelRequestAsync(Guid id, string token)
    {
        string newToken = token.Replace(" ", "+");
        bool correct = await _userService.VerifyUserToken(newToken);
        if (!correct) throw new UnauthorizedAccessException("invalid token");
        return await CancellationRequestOfProductAsync(id);
    }

    public async Task<Result<Guid>> RemoveCancellationRequest(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.Status = ProductStatus.Active;
        product.TerminationDate = null;
        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();
        await SendEmailForProductStatusChangeAsync(product);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> CancellationRequestEOB(Guid id)
    {
        var product = await _repository.GetByIdAsync<Product>(id);
        if (product == null) throw new EntityNotFoundException(string.Format(_localizer["product.notfound"], id));

        product.TerminationDate = product.NextDueDate?.Subtract(TimeSpan.FromDays(1));

        product.DomainEvents.Add(new ProductUpdatedEvent(product));

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();
        await SendEmailForProductStatusChangeAsync(product);

        return await Result<Guid>.SuccessAsync(id);
    }

    private async Task<bool> SendEmailForProductStatusChangeAsync(Product product)
    {
        var user = await _userService.GetAsync(product.AssignedToClientId);
        if (user == null) return false;
        var emailBody = await _templateService.GenerateEmailBodyForProductStatusUpdate(product, user.Data);
        var mailRequest = new MailRequest
        {
            From = _mailSettings.From,
            To = new List<string> { user.Data.Email },
            Body = emailBody.Body,
            Subject = emailBody.Subject
        };
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));

        string message = $"Hello [[fullName]], the product status has updated.";
        /*await _notificationService.SendMessageToAdminsHavingPermissionAsync(
            new BasicNotification()
            {
                Message = message,
                Label = BasicNotification.LabelType.Information,
                NotificationType = NotificationType.PRODUCT_STATUS_UPDATED,
                TargetUserTypes = NotificationTargetUserTypes.Admins,
                Data = product
            },
            PermissionConstants.Products.Update);*/

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.PRODUCT_STATUS_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = product.Id, Data = new { product = product } });
        return true;
    }
    #endregion

    private bool GetUserPermision(List<UserModuleDto> modules, string module)
    {
        var permissionForThisController = modules.FirstOrDefault(m => m.Name.Equals(module, StringComparison.OrdinalIgnoreCase));
        if (permissionForThisController == null)
            return false;
        var permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissionForThisController.PermissionDetail);
        return permissions.TryGetValue("View", out bool isActive) && isActive;
    }
}
