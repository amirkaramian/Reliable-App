using Microsoft.Extensions.Localization;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ManageUserApiKey;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using MyReliableSite.Shared.DTOs.ManageSubUserApiKey;
using MyReliableSite.Application.Specifications;
using Newtonsoft.Json.Linq;
using MyReliableSite.Application.Settings;
using Newtonsoft.Json;
using Mapster;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.ManageUserApiKey.Services;

public class APIKeyPairService : IAPIKeyPairService
{
    private readonly IStringLocalizer<APIKeyPairService> _localizer;
    private readonly IRepositoryAsync _repository;

    public APIKeyPairService()
    {
    }

    public APIKeyPairService(IRepositoryAsync repository, IStringLocalizer<APIKeyPairService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateAPIKeyPairAsync(CreateAPIKeyPairRequest request)
    {
        bool aPIKeyPairExists = await _repository.ExistsAsync<APIKeyPair>(a => a.ApplicationKey == request.ApplicationKey);
        if (aPIKeyPairExists) throw new EntityAlreadyExistsException(string.Format(_localizer["ApplicationKey.alreadyexists"], request.ApplicationKey));
        var aPIKeyPair = new APIKeyPair(request.ApplicationKey, request.UserIds, request.SafeListIpAddresses, request.ValidTill, request.StatusApi, request.Label);

        aPIKeyPair.UserApiKeyModules = new List<UserApiKeyModule>();

        aPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        var aPIKeyPairId = await _repository.CreateAsync<APIKeyPair>((APIKeyPair)aPIKeyPair);
        await _repository.SaveChangesAsync();

        foreach (var item in request.UserApiKeyModules)
        {
            aPIKeyPair.UserApiKeyModules.Add(new UserApiKeyModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, aPIKeyPairId));
        }

        _ = await _repository.CreateRangeAsync<UserApiKeyModule>(aPIKeyPair.UserApiKeyModules);
        _ = await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(aPIKeyPairId);
    }

    public async Task<Result<Guid>> CreateSubUserAPIKeyPairAsync(CreateSubUserAPIKeyPairRequest request)
    {
        bool aPIKeyPairExists = await _repository.ExistsAsync<APIKeyPair>(a => a.ApplicationKey == request.ApplicationKey);
        if (aPIKeyPairExists) throw new EntityAlreadyExistsException(string.Format(_localizer["ApplicationKey.alreadyexists"], request.ApplicationKey));
        var aPIKeyPair = new APIKeyPair(request.ApplicationKey, request.SubUserIds, request.SafeListIpAddresses, request.ValidTill, request.StatusApi, request.Label);

        aPIKeyPair.UserApiKeyModules = new List<UserApiKeyModule>();

        aPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        var aPIKeyPairId = await _repository.CreateAsync<APIKeyPair>((APIKeyPair)aPIKeyPair);
        await _repository.SaveChangesAsync();

        foreach (var item in request.SubUserApiKeyModules)
        {
            aPIKeyPair.UserApiKeyModules.Add(new UserApiKeyModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, aPIKeyPairId));
        }

        _ = await _repository.CreateRangeAsync<UserApiKeyModule>(aPIKeyPair.UserApiKeyModules);
        _ = await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(aPIKeyPairId);
    }

    public async Task<Result<Guid>> DeleteAPIKeyPairAsync(Guid id)
    {
        var aPIKeyPairToDelete = await _repository.RemoveByIdAsync<APIKeyPair>(id);
        aPIKeyPairToDelete.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<APIKeyPairDto>> SearchAsync(APIKeyPairListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<APIKeyPair, APIKeyPairDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<IEnumerable<APIKeyPairDto>>> GetAllAsync()
    {
        return await Result<IEnumerable<APIKeyPairDto>>.SuccessAsync((await _repository.GetListAsync<APIKeyPair>(_ => true, true))?.Select(s => s.Adapt<APIKeyPairDto>()));
    }

    public async Task<Result<Guid>> UpdateAPIKeyPairAsync(UpdateAPIKeyPairRequest request, Guid id)
    {
        var aPIKeyPair = await _repository.GetByIdAsync<APIKeyPair>(id);
        if (aPIKeyPair == null) throw new EntityNotFoundException(string.Format(_localizer["ApplicationKey.notfound"], id));
        var updatedAPIKeyPair = aPIKeyPair.Update(request.ApplicationKey, request.UserIds, request.SafeListIpAddresses, request.ValidTill, request.StatusApi, request.Label);
        updatedAPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<APIKeyPair>(updatedAPIKeyPair);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateSubUserAPIKeyPairAsync(UpdateSubUserAPIKeyPairRequest request, Guid id)
    {
        var aPIKeyPair = await _repository.GetByIdAsync<APIKeyPair>(id);
        if (aPIKeyPair == null) throw new EntityNotFoundException(string.Format(_localizer["ApplicationKey.notfound"], id));
        var updatedAPIKeyPair = aPIKeyPair.Update(request.ApplicationKey, request.SubUserIds, request.SafeListIpAddresses, request.ValidTill, request.StatusApi, request.Label);
        updatedAPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<APIKeyPair>(updatedAPIKeyPair);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateAPIKeyPairPermissionsAsync(UpdateAPIKeyPairPermissionRequest request, Guid id)
    {
        var aPIKeyPair = await _repository.GetByIdAsync<APIKeyPair>(id);
        if (aPIKeyPair == null) throw new EntityNotFoundException(string.Format(_localizer["ApplicationKey.notfound"], id));

        await _repository.ClearAsync<UserApiKeyModule>(m => m.APIKeyPairId == id);

        await _repository.SaveChangesAsync();
        if (aPIKeyPair.UserApiKeyModules == null)
        {
            aPIKeyPair.UserApiKeyModules = new List<UserApiKeyModule>();
        }
        else
        {
            aPIKeyPair.UserApiKeyModules.Clear();
        }

        foreach (var item in request.UserApiKeyModules)
        {
            aPIKeyPair.UserApiKeyModules.Add(new UserApiKeyModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, id));
        }

        if (aPIKeyPair.UserApiKeyModules != null && aPIKeyPair.UserApiKeyModules.Count > 0)
        {
            _ = await _repository.CreateRangeAsync<UserApiKeyModule>(aPIKeyPair.UserApiKeyModules);
        }

        aPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<APIKeyPair>(aPIKeyPair);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateSubUserAPIKeyPairPermissionsAsync(UpdateSubUserAPIKeyPairPermissionRequest request, Guid id)
    {
        var aPIKeyPair = await _repository.GetByIdAsync<APIKeyPair>(id);
        if (aPIKeyPair == null) throw new EntityNotFoundException(string.Format(_localizer["ApplicationKey.notfound"], id));

        await _repository.ClearAsync<UserApiKeyModule>(m => m.APIKeyPairId == id);

        await _repository.SaveChangesAsync();
        if (aPIKeyPair.UserApiKeyModules == null)
        {
            aPIKeyPair.UserApiKeyModules = new List<UserApiKeyModule>();
        }
        else
        {
            aPIKeyPair.UserApiKeyModules.Clear();
        }

        foreach (var item in request.UserApiKeyModules)
        {
            aPIKeyPair.UserApiKeyModules.Add(new UserApiKeyModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, id));
        }

        if (aPIKeyPair.UserApiKeyModules != null && aPIKeyPair.UserApiKeyModules.Count > 0)
        {
            _ = await _repository.CreateRangeAsync<UserApiKeyModule>(aPIKeyPair.UserApiKeyModules);
        }

        aPIKeyPair.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<APIKeyPair>(aPIKeyPair);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<APIKeyPairDto>> GetAPIKeyPairAsync(Guid id)
    {
        var spec = new BaseSpecification<APIKeyPair>();
        spec.Includes.Add(a => a.UserApiKeyModules);
        var aPIKeyPair = await _repository.GetByIdAsync<APIKeyPair, APIKeyPairDto>(id, spec);
        return await Result<APIKeyPairDto>.SuccessAsync(aPIKeyPair);
    }

    public async Task<Result<APIKeyPairDto>> GetAPIKeyPairAsync(string appName)
    {
        var aPIKeyPair = await _repository.QueryFirstOrDefaultAsync<APIKeyPair>($"SELECT * FROM dbo.\"APIKeyPairs\" WHERE \"ApplicationKey\"  = '{appName}'");
        var mappedArticle = aPIKeyPair.Adapt<APIKeyPairDto>();
        return await Result<APIKeyPairDto>.SuccessAsync(mappedArticle);
    }

    public async Task<Result<List<APIKeyPairEXL>>> GetAPIKeyPairListAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var apikeypairs = await _repository.QueryWithDtoAsync<APIKeyPairEXL>($@"SELECT AKP.*
                                                                                                        FROM APIKeyPairs AKP
                                                                                                        WHERE ((CONVERT(date, [AKP].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [AKP].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and UserIds = '{userId}' ORDER BY AKP.CreatedOn ASC");
        return await Result<List<APIKeyPairEXL>>.SuccessAsync(apikeypairs.ToList());
    }

}
