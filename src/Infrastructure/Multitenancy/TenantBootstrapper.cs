using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Settings;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Multitenancy;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Infrastructure.Utilities;
using MySqlConnector;
using Npgsql;
using Serilog;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MyReliableSite.Infrastructure.Multitenancy;

public class TenantBootstrapper
{
    private static readonly ILogger _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    public static void Initialize(ApplicationDbContext appContext, DatabaseSettings options, Tenant tenant, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IList<IDatabaseSeeder> seeders)
    {
        string connectionString = string.IsNullOrEmpty(tenant.ConnectionString) ? options.ConnectionString : tenant.ConnectionString;
        bool isValid = TryValidateConnectionString(options, connectionString, tenant.Key);
        if (isValid)
        {
            appContext.Database.SetConnectionString(connectionString);
            if (appContext.Database.GetMigrations().Any())
            {
                if (appContext.Database.GetPendingMigrations().Any())
                {
                    _logger.Information($"Applying Migrations for '{tenant.Key}' tenant.");
                    appContext.Database.Migrate();
                }

                if (appContext.Database.CanConnect())
                {
                    _logger.Information($"Connection to {tenant.Key}'s Database Succeeded.");
                    SeedRolesAsync(tenant, roleManager, appContext).GetAwaiter().GetResult();
                    if (tenant.Name.ToLower() == "admin")
                    {
                        SeedTenantAdminAsync(tenant, userManager, roleManager, appContext).GetAwaiter().GetResult();
                    }
                    else if (tenant.Name.ToLower() == "client")
                    {
                        SeedTenantClientAsync(tenant, userManager, roleManager, appContext).GetAwaiter().GetResult();
                    }
                }

                foreach (var seeder in seeders)
                {
                    seeder.Initialize();
                }
            }
        }
    }

    private static async Task SeedRolesAsync(Tenant tenant, RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        foreach (string roleName in typeof(RoleConstants).GetAllPublicConstantValues<string>())
        {
            var roleStore = new RoleStore<ApplicationRole>(applicationDbContext);
            var role = new ApplicationRole(roleName, tenant.Key, $"{roleName} Role for {tenant.Key} Tenant");

            if (!string.IsNullOrEmpty(tenant.Key) && tenant.Key.ToLower() == "admin")
            {
                if (roleName == RoleConstants.Admin || roleName == RoleConstants.SuperAdmin)
                {
                    if (!await applicationDbContext.Roles.IgnoreQueryFilters().AnyAsync(r => r.Name == roleName && r.Tenant == tenant.Key))
                    {
                        await roleStore.CreateAsync(role);
                        _logger.Information($"Seeding {roleName} Role for '{tenant.Key}' Tenant.");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(tenant.Key) && tenant.Key.ToLower() == "client")
            {
                if (roleName == RoleConstants.Client)
                {
                    if (!await applicationDbContext.Roles.IgnoreQueryFilters().AnyAsync(r => r.Name == roleName && r.Tenant == tenant.Key))
                    {
                        await roleStore.CreateAsync(role);
                        _logger.Information($"Seeding {roleName} Role for '{tenant.Key}' Tenant.");
                    }

                    var basicRole = await roleManager.Roles.IgnoreQueryFilters()
                    .Where(a => a.NormalizedName == RoleConstants.Client.ToUpper() && a.Tenant == tenant.Key)
                    .FirstOrDefaultAsync();
                    var basicClaims = await roleManager.GetClaimsAsync(basicRole);
                    foreach (string permission in DefaultPermissions.Clients)
                    {
                        if (!basicClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == permission))
                        {
                            await roleManager.AddClaimAsync(basicRole, new Claim(ClaimConstants.Permission, permission));
                            _logger.Information($"Seeding Client Permission '{permission}' for '{tenant.Key}' Tenant.");
                        }
                    }
                }
            }
        }
    }

    private static async Task SeedTenantAdminAsync(Tenant tenant, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        string adminUserName = $"{tenant.Key.Trim()}.{RoleConstants.Admin}".ToLower();
        var superUser = new ApplicationUser
        {
            FirstName = tenant.Key.Trim().ToLower(),
            LastName = RoleConstants.Admin,
            Email = tenant.AdminEmail,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = tenant.AdminEmail.ToUpper(),
            NormalizedUserName = adminUserName.ToUpper(),
            IsActive = true,
            Status = true,
            Tenant = tenant.Key.Trim().ToLower()
        };
        if (!applicationDbContext.Users.IgnoreQueryFilters().Any(u => u.Email == tenant.AdminEmail))
        {
            var password = new PasswordHasher<ApplicationUser>();
            superUser.PasswordHash = password.HashPassword(superUser, MultitenancyConstants.DefaultPassword);
            var userStore = new UserStore<ApplicationUser>(applicationDbContext);
            await userStore.CreateAsync(superUser);
            _logger.Information($"Seeding Default Admin User for '{tenant.Key}' Tenant.");
        }

        await AssignAdminRoleAsync(superUser.Email, tenant.Key, applicationDbContext, userManager, roleManager);
    }

    private static async Task SeedTenantClientAsync(Tenant tenant, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        string clientUserName = $"{tenant.Key.Trim()}.{RoleConstants.Client}".ToLower();
        var superUser = new ApplicationUser
        {
            FirstName = tenant.Key.Trim().ToLower(),
            LastName = RoleConstants.Client,
            Email = "admin@MyReliableSite.com",
            UserName = clientUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = "ADMIN@MYRELIABLESITE.COM",
            NormalizedUserName = clientUserName.ToUpper(),
            IsActive = true,
            Status = true,
            Tenant = tenant.Key.Trim().ToLower()
        };
        if (!applicationDbContext.Users.IgnoreQueryFilters().Any(u => u.Email == tenant.AdminEmail))
        {
            var password = new PasswordHasher<ApplicationUser>();
            superUser.PasswordHash = password.HashPassword(superUser, MultitenancyConstants.DefaultPassword);
            var userStore = new UserStore<ApplicationUser>(applicationDbContext);
            await userStore.CreateAsync(superUser);
            _logger.Information($"Seeding Default Client User for '{tenant.Key}' Tenant.");
        }

        await AssignClientRoleAsync(superUser.Email, tenant.Key, applicationDbContext, userManager, roleManager);
    }

    public static async Task AssignAdminRoleAsync(string email, string tenant, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var user = await userManager.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.Equals(email));

        if (user == null) return;

        var roleRecord = await roleManager.Roles.IgnoreQueryFilters()
            .Where(a => a.NormalizedName == RoleConstants.Admin.ToUpper() && a.Tenant == tenant)
            .FirstOrDefaultAsync();

        if (roleRecord == null) return;

        bool isUserInRole = await applicationDbContext.UserRoles.AnyAsync(a => a.UserId == user.Id && a.RoleId == roleRecord.Id);
        if (!isUserInRole)
        {
            applicationDbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = roleRecord.Id, UserId = user.Id });
            await applicationDbContext.SaveChangesAsync();
            _logger.Information($"Assigning Admin Permissions for '{tenant}' Tenant.");
        }

        var allClaims = await roleManager.GetClaimsAsync(roleRecord);
        foreach (string permission in typeof(PermissionConstants).GetNestedClassesStaticStringValues())
        {
            if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == permission))
            {
                await roleManager.AddClaimAsync(roleRecord, new Claim(ClaimConstants.Permission, permission));
            }
        }

        if (tenant == MultitenancyConstants.Root.Key && email == MultitenancyConstants.Root.EmailAddress)
        {
            foreach (string rootPermission in typeof(RootPermissions).GetNestedClassesStaticStringValues())
            {
                if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == rootPermission))
                {
                    await roleManager.AddClaimAsync(roleRecord, new Claim(ClaimConstants.Permission, rootPermission));
                }
            }
        }

        await applicationDbContext.SaveChangesAsync();
    }

    public static async Task AssignClientRoleAsync(string email, string tenant, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var user = await userManager.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.Equals(email));
        if (user == null) return;
        var roleRecord = await roleManager.Roles.IgnoreQueryFilters()
            .Where(a => a.NormalizedName == RoleConstants.Client.ToUpper() && a.Tenant == tenant)
            .FirstOrDefaultAsync();
        if (roleRecord == null) return;
        bool isUserInRole = await applicationDbContext.UserRoles.AnyAsync(a => a.UserId == user.Id && a.RoleId == roleRecord.Id);
        if (!isUserInRole)
        {
            applicationDbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = roleRecord.Id, UserId = user.Id });
            await applicationDbContext.SaveChangesAsync();
            _logger.Information($"Assigning Client Permissions for '{tenant}' Tenant.");
        }

        var allClaims = await roleManager.GetClaimsAsync(roleRecord);
        foreach (string permission in typeof(PermissionConstants).GetNestedClassesStaticStringValues())
        {
            if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == permission))
            {
                await roleManager.AddClaimAsync(roleRecord, new Claim(ClaimConstants.Permission, permission));
            }
        }

        if (tenant == MultitenancyConstants.Root.Key && email == MultitenancyConstants.Root.EmailAddress)
        {
            foreach (string rootPermission in typeof(RootPermissions).GetNestedClassesStaticStringValues())
            {
                if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == rootPermission))
                {
                    await roleManager.AddClaimAsync(roleRecord, new Claim(ClaimConstants.Permission, rootPermission));
                }
            }
        }

        await applicationDbContext.SaveChangesAsync();
    }

    public static bool TryValidateConnectionString(DatabaseSettings options, string connectionString, string key)
    {
        try
        {
            switch (options.DBProvider)
            {
                case "postgresql":
                    var postgresqlcs = new NpgsqlConnectionStringBuilder(connectionString);
                    break;

                case "mysql":
                    var mysqlcs = new MySqlConnectionStringBuilder(connectionString);
                    break;

                case "mssql":
                    var mssqlcs = new SqlConnectionStringBuilder(connectionString);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"{key} Connection String Exception : {ex.Message}");
            return false;
        }
    }
}
