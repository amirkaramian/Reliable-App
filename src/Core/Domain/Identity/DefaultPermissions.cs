namespace MyReliableSite.Domain.Constants;

public static class DefaultPermissions
{
    public static List<string> Basics => new()
    {
        PermissionConstants.Articles.Search,
        PermissionConstants.Articles.View,
        PermissionConstants.Categories.Search,
        PermissionConstants.Categories.View
    };

    public static List<string> Clients => new()
    {
        PermissionConstants.Articles.Search,
        PermissionConstants.Articles.View,
        PermissionConstants.Categories.Search,
        PermissionConstants.Categories.View,
        PermissionConstants.Identity.Register,
        PermissionConstants.Roles.Register
    };
}
