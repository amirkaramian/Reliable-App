using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Brands.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.Brands;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Brands.Services;

public class BrandService : IBrandService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<BrandService> _localizer;
    private readonly IFileStorageService _fileStorageService;

    private readonly IUserService _userService;
    public BrandService(IRepositoryAsync repository, IUserService userService, IStringLocalizer<BrandService> localizer, IFileStorageService fileStorageService)
    {
        _userService = userService;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    }

    public async Task<Result<BrandDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<Brand>(id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], id));
        BrandDto brandDto = toReturn.Adapt<BrandDto>();
        brandDto.Base64Logo = await _fileStorageService.ReturnBase64StringOfImageFileAsync(toReturn.LogoUrl);
        return await Result<BrandDto>.SuccessAsync(brandDto);
    }

    public async Task<Result<List<UserDetailsDto>>> GetBrandUsersAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<Brand>(id);
        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], id));
        return await _userService.GetAllAsync(toReturn.ClientAssigned.Split(","));
    }

    public async Task<PaginatedResult<BrandDto>> SearchAsync(BrandListFilter filter)
    {
        var brands = await _repository.GetSearchResultsAsync<Brand, BrandDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, new Filters<Brand>(), filter.AdvancedSearch, filter.Keyword);
        foreach (var item in brands.Data)
        {
            item.Base64Logo = item.LogoUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(item.LogoUrl);
        }

        return brands;
    }

    public async Task<Result<BrandDto>> CreateAsync(CreateBrandRequest request)
    {
        if (await _repository.ExistsAsync<Brand>(a => a.CompanyName.ToLower().Equals(request.CompanyName.ToLower())))
            throw new EntityAlreadyExistsException(string.Format(_localizer["brand.alreadyexists"], request.CompanyName));

        var toAdd = new Brand(_fileStorageService.GetBase64Image(request.Image), request.CompanyName, request.Status, request.Name, request.ClientAssigned, request.IsDefault, request.TermsOfServiceURL, request.TermsOfServiceAgreement, request.Address);

        toAdd.DomainEvents.Add(new BrandCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();
        return await Result<BrandDto>.SuccessAsync(toAdd.Adapt<BrandDto>());
    }

    public async Task<Result<BrandDto>> UpdateAsync(Guid id, UpdateBrandRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<Brand>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], id));

        var updatedBrand = toUpdate.Update(request.Image != null ? _fileStorageService.GetBase64Image(request.Image) : null, request.CompanyName, request.Status, request.Name, request.ClientAssigned, request.IsDefault, request.TermsOfServiceURL, request.TermsOfServiceAgreement, request.Address);

        toUpdate.DomainEvents.Add(new BrandUpdatedEvent(toUpdate));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedBrand);
        _ = await _repository.SaveChangesAsync();
        return await Result<BrandDto>.SuccessAsync(updatedBrand.Adapt<BrandDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var brand = await _repository.GetByIdAsync<Brand>(id);

        if (brand is null) throw new EntityNotFoundException(string.Format(_localizer["Brand.notfound"], id));

        if (brand.IsDefault)
        {
            throw new InvalidOperationException(string.Format(_localizer["This brand is default, can't be deleted"]));
        }

        var defaultBrand = await _repository.FirstByConditionAsync<Brand>(o => o.IsDefault);

        if (defaultBrand is not null)
        {
            var departments = await _repository.GetListAsync<Department>(o => o.BrandId == id);

            foreach (var item in departments)
            {
                item.BrandId = defaultBrand.Id;
                defaultBrand.Departments.Add(item);
            }
        }

        var toDelete = await _repository.RemoveByIdAsync<Brand>(id);

        toDelete.DomainEvents.Add(new BrandDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }

    public async Task<Result<BrandLogoutDto>> GetBrandLogout(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<Brand>(id);

        if (toReturn == null)
        {
            var defaultBrand = await _repository.GetByIdAsync<Brand>(Guid.Parse("75B5C596-E940-4CC5-8C0D-D4556FE4C4C8"));
            toReturn = defaultBrand;
        }

        BrandLogoutDto brandDto = toReturn.Adapt<BrandLogoutDto>();
        brandDto.Base64Logo = toReturn.LogoUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(toReturn.LogoUrl);
        return await Result<BrandLogoutDto>.SuccessAsync(brandDto);
    }
}
