using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class UserLoginHistoryService : IUserLoginHistoryService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UserLoginHistoryService> _localizer;
    private readonly IRepositoryAsync _repository;

    public UserLoginHistoryService()
    {
    }

    public UserLoginHistoryService(IRepositoryAsync repository, IStringLocalizer<UserLoginHistoryService> localizer, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateUserLoginHistoryAsync(CreateUserLoginHistoryRequest request)
    {
        var userLoginHistory = new MyReliableSite.Domain.Identity.UserLoginHistory(request.UserId, request.LoginTime, request.IpAddress, request.DeviceName, request.Location, (MyReliableSite.Domain.Identity.UserLoginStatus)request.Status);
        userLoginHistory.DomainEvents.Add(new StatsChangedEvent());
        var userLoginHistoryId = await _repository.CreateAsync<MyReliableSite.Domain.Identity.UserLoginHistory>((UserLoginHistory)userLoginHistory);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(userLoginHistoryId);

    }

    public async Task<PaginatedResult<UserLoginHistoryDto>> SearchAsync(UserLoginHistoryListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<UserLoginHistory, UserLoginHistoryDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<UserLoginHistoryDto>> GetUserLoginHistoryAsync(Guid id)
    {
        var spec = new BaseSpecification<UserLoginHistory>();
        var userLoginHistory = await _repository.GetByIdAsync<UserLoginHistory, UserLoginHistoryDto>(id, spec);
        return await Result<UserLoginHistoryDto>.SuccessAsync(userLoginHistory);
    }

    public async Task<Result<List<UserLoginHistoryDto>>> GetUserLoginHistoryByUserIdAsync(string userId)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();
        var spec = new BaseSpecification<UserLoginHistory>();
        var userLoginHistory = await _repository.GetListAsync<UserLoginHistory>(m => m.UserId == userId);
        var dto = userLoginHistory.Adapt<List<UserLoginHistoryDto>>();
        dto.ForEach(x => x.FullName = user.FullName);

        return await Result<List<UserLoginHistoryDto>>.SuccessAsync(dto);
    }
}