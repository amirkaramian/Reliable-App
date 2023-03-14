using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Abstractions.Services.Identity;

public interface IUserService : ITransientService
{
    Task<PaginatedResult<UserDetailsDto>> SearchAsync(UserListFilter filter);
    Task<PaginatedResult<UserDetailsDto>> SearchSubUsersAsync(UserListFilter filter);

    Task<Result<List<UserDetailsDto>>> GetAllAsync(IEnumerable<string> ids);
    Task<Result<List<UserDetailsDto>>> GetAllUsersofAdminGroupAsync(string adminGroupId);
    Task<Result<List<UserDetailsDto>>> GetAllAsync();

    Task<Result<List<UserDetailsDto>>> GetAllSubUsersAsync();

    Task<Result<List<UserDetailsDto>>> GetAllSubUsersByClientIDAsync(string clientId);

    Task<Result<List<UserDetailsDto>>> GetAllByUserRoleAsync(string roleName);

    Task<Result<List<UserDetailsDto>>> GetAllUsersToTakeOrdersAsync();

    Task<int> GetCountAsync();

    Task<IResult<UserDetailsDto>> GetAsync(string userId);

    Task<IResult<UserDetailsDto>> GetSubUserAsync(string userId);

    Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

    Task<IResult<UserRolesResponse>> GetSubUserRolesAsync(string userId);

    Task<IResult<string>> AssignRolesAsync(string userId, UserRolesRequest request);

    Task<IResult<string>> AssignSubUserRolesAsync(string userId, UserRolesRequest request);

    Task<Result<List<PermissionDto>>> GetPermissionsAsync(string id);
    Task<Result<List<PermissionDto>>> GetSubUserPermissionsAsync(string userId);
    Task<Result<Guid>> DeleteUserAsync(Guid userId);
    Task<Result<Guid>> DeleteUserAccountAsync();
    Task<IResult<UserDetailsDto>> GetUserProfileAsync();
    Task<IResult<UserDetailsDto>> GetUserProfileAsync(string id);
    Task<IResult<UserDetailsDto>> GetUserProfileByEmailAsync(string email);
    Task<IResult<UserDetailsDto>> GetUserProfileByUsernameAsync(string username);

    Task<IResult<UserDetailsDto>> GetUserProfileByFullNameAsync(string userFullName, int oldUserId);
    Task<IResult<string>> ActiveUserAsync(string userId);
    Task<IResult<string>> DeactiveUserAsync(string userId);
    Task<IResult<string>> ActivateUserTakeOrder(string userId);
    Task<IResult<string>> DeActivateUserTakeOrder(string userId);
    Task<IResult<string>> ActivateUserAvailableForOrders(string userId);
    Task<IResult<string>> DeActivateUserAvailableForOrders(string userId);
    Task<Result<List<UserDetailsDto>>> GetUsersBasedOnConditionsForNotificationTemplates(UsersBasedOnConditionsRequest usersBasedOnConditionsDto);
    Task<IResult> CreateSubUserAsync(CreateSubUserRequest request, string origin);

    Task<IResult> UpdateSubUserAsync(CreateSubUserRequest request, string subUserId, string origin);
    Task<string> GenerateUserToken(string purpose = "General");
    Task<bool> VerifyUserToken(string token, string purpose = "General");

    Task<Result<List<UserDetailsEXL>>> GetSubUserListAsync(string userId, DateTime startDate, DateTime endDate);
    Task MailAdminUser(string messege, string subject);
}