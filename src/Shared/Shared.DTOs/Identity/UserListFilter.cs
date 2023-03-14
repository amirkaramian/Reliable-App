using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.Identity;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}