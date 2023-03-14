using Mapster;

using Microsoft.Extensions.Localization;

using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.SmtpConfigurations.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.SmtpConfigurations;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.Services;

public class SmtpConfigurationService : ISmtpConfigurationService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<SmtpConfigurationService> _localizer;

    public SmtpConfigurationService(IRepositoryAsync repository, IStringLocalizer<SmtpConfigurationService> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Result<SmtpConfigurationDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<SmtpConfiguration>(id);

        toReturn.BrandSmtpConfigurations = await _repository.GetListAsync<BrandSmtpConfiguration>(m => m.SmtpConfigurationId == toReturn.Id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["smtpconfiguration.notfound"], id));

        return await Result<SmtpConfigurationDto>.SuccessAsync(toReturn.Adapt<SmtpConfigurationDto>());
    }

    public async Task<Result<SmtpConfigurationDto>> GetByBrandAsync(Guid brandId)
    {
        var toReturn = await _repository.GetListAsync<SmtpConfiguration>(s => s.BrandId == brandId);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["smtpconfiguration.notfoundforbrand"], brandId));

        foreach (var item in toReturn)
        {
            item.BrandSmtpConfigurations = await _repository.GetListAsync<BrandSmtpConfiguration>(m => m.SmtpConfigurationId == item.Id);
        }

        return await Result<SmtpConfigurationDto>.SuccessAsync(toReturn.Adapt<SmtpConfigurationDto>());
    }

    public async Task<Result<IEnumerable<SmtpConfigurationDto>>> GetAllAsync()
    {
        return await Result<IEnumerable<SmtpConfigurationDto>>.SuccessAsync((await _repository.GetListAsync<SmtpConfiguration>(_ => true, true))?.Select(s => s.Adapt<SmtpConfigurationDto>()));
    }

    public async Task<PaginatedResult<SmtpConfigurationDto>> SearchAsync(SmtpConfigurationListFilter filter)
    {
        var reps = await _repository.GetSearchResultsAsync<SmtpConfiguration, SmtpConfigurationDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, new Filters<SmtpConfiguration>(), filter.AdvancedSearch, filter.Keyword);

        foreach (var item in reps.Data)
        {
            var toReturn = await _repository.GetListAsync<BrandSmtpConfiguration>(m => m.SmtpConfigurationId == item.Id);
            item.BrandSmtpConfigurations = toReturn.Adapt<List<BrandSmtpConfigurationDto>>();
        }

        return reps;
    }

    public async Task<Result<SmtpConfigurationDto>> CreateAsync(CreateSmtpConfigurationRequest request)
    {
        var toAdd = new SmtpConfiguration(request.Port, request.HttpsProtocol, request.Host, request.Username, request.Password, request.FromName, request.FromEmail, request.Signature, request.CssStyle, request.HeaderContent, request.FooterContent, request.CompanyAddress, request.Bcc);

        foreach (var brand in request.BrandSmtpConfigurations)
        {
            var existingBrand = await _repository.GetByIdAsync<Brand>(brand.BrandId);

            if (existingBrand == null)
                throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brand.BrandId));

            toAdd.BrandSmtpConfigurations.Add(new BrandSmtpConfiguration() { Brand = existingBrand, BrandId = brand.BrandId, DepartmentId = brand.DepartmentId });
        }

        toAdd.DomainEvents.Add(new SmtpConfigurationCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();

        /*foreach (var item in toAdd.BrandSmtpConfigurations)
        {
            item.SmtpConfigurationId = toAdd.Id;
            item.SmtpConfiguration = toAdd;
        }

        _ = await _repository.CreateRangeAsync<BrandSmtpConfiguration>(toAdd.BrandSmtpConfigurations);
        _ = await _repository.SaveChangesAsync();*/

        return await Result<SmtpConfigurationDto>.SuccessAsync(toAdd.Adapt<SmtpConfigurationDto>());
    }

    public async Task<Result<SmtpConfigurationDto>> UpdateAsync(Guid id, UpdateSmtpConfigurationRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<SmtpConfiguration>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["smtpconfiguration.notfound"], id));

        var updatedSmtpConfiguration = toUpdate.Update(request.Port, request.HttpsProtocol, request.Host, request.Username, request.Password, request.FromName, request.FromEmail, request.Signature, request.CssStyle, request.HeaderContent, request.FooterContent, request.CompanyAddress, request.Bcc);

        toUpdate.DomainEvents.Add(new SmtpConfigurationUpdatedEvent(updatedSmtpConfiguration));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedSmtpConfiguration);
        _ = await _repository.SaveChangesAsync();

        if (updatedSmtpConfiguration.BrandSmtpConfigurations != null && updatedSmtpConfiguration.BrandSmtpConfigurations.Count > 0)
        {
            await _repository.ClearAsync<Domain.Billing.BrandSmtpConfiguration>(m => m.SmtpConfigurationId == id && m.Tenant == request.Tenant);
        }

        foreach (var brandSmtpConfiguration in request.BrandSmtpConfigurations)
        {
            var brand = await _repository.GetByIdAsync<Brand>(brandSmtpConfiguration.BrandId);

            if (brand == null)
                throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brandSmtpConfiguration.BrandId));

            if (updatedSmtpConfiguration.BrandSmtpConfigurations == null)
            {
                updatedSmtpConfiguration.BrandSmtpConfigurations = new List<BrandSmtpConfiguration>();
            }

            updatedSmtpConfiguration.BrandSmtpConfigurations.Add(new BrandSmtpConfiguration()
            {
                SmtpConfiguration = updatedSmtpConfiguration,
                SmtpConfigurationId = updatedSmtpConfiguration.Id,
                Brand = brand,
                BrandId = brandSmtpConfiguration.BrandId,
                DepartmentId = brandSmtpConfiguration.DepartmentId
            });
        }

        await _repository.CreateRangeAsync<BrandSmtpConfiguration>(updatedSmtpConfiguration.BrandSmtpConfigurations);
        return await Result<SmtpConfigurationDto>.SuccessAsync(updatedSmtpConfiguration.Adapt<SmtpConfigurationDto>());
    }

    public async Task<Result<SmtpConfigurationDto>> UpdateForBrandAsync(Guid brandId, Guid departmentId, UpdateSmtpConfigurationRequest request)
    {
        var brand = await _repository.GetByIdAsync<Brand>(brandId);

        if (brand == null)
            throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brandId));

        var toUpdateBrandSmtp = await _repository.FirstByConditionAsync<BrandSmtpConfiguration>(s => s.BrandId == brandId && s.DepartmentId == departmentId);
        var toUpdate = await _repository.FirstByConditionAsync<SmtpConfiguration>(s => s.Id == toUpdateBrandSmtp.SmtpConfigurationId);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["smtpconfiguration.notfoundforbrand"], brandId));

        var updatedSmtpConfiguration = toUpdate.Update(request.Port, request.HttpsProtocol, request.Host, request.Username, request.Password, request.FromName, request.FromEmail, request.Signature, request.CssStyle, request.HeaderContent, request.FooterContent, request.CompanyAddress, request.Bcc);

        toUpdate.DomainEvents.Add(new SmtpConfigurationUpdatedEvent(updatedSmtpConfiguration));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedSmtpConfiguration);
        _ = await _repository.SaveChangesAsync();
        return await Result<SmtpConfigurationDto>.SuccessAsync(updatedSmtpConfiguration.Adapt<SmtpConfigurationDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.RemoveByIdAsync<SmtpConfiguration>(id);

        toDelete.DomainEvents.Add(new SmtpConfigurationDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }

    public async Task<Result<bool>> DeleteForBrandAsync(Guid brandId)
    {
        var brand = await _repository.GetByIdAsync<Brand>(brandId);

        if (brand == null)
            throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brandId));

        await _repository.ClearAsync<SmtpConfiguration>(s => s.BrandId == brandId);

        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }
}
