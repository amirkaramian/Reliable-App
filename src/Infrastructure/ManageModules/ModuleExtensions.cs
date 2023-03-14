using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyReliableSite.Application.Settings;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using Newtonsoft.Json;
using Serilog;

namespace MyReliableSite.Infrastructure.ManageModules;

public static class ModuleExtensions
{
    private static readonly ILogger _logger = Log.ForContext(typeof(ModuleExtensions));

    public static IServiceCollection AddModules<TA>(this IServiceCollection services, IConfiguration config)
    where TA : ApplicationDbContext
    {
        services.Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)));

        // services.Configure<ModuleManagementSettings>(config.GetSection(nameof(ModuleManagementSettings)));

        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        var moduleSettings = services.GetOptions<ModuleManagementSettings>(nameof(ModuleManagementSettings));

        string rootConnectionString = databaseSettings.ConnectionString;
        string dbProvider = databaseSettings.DBProvider;
        if (string.IsNullOrEmpty(dbProvider)) throw new Exception("DB Provider is not configured.");
        _logger.Information($"Current DB Provider : {dbProvider}");
        switch (dbProvider.ToLower())
        {
            case "mssql":
                services.AddDbContext<TA>(m => m.UseSqlServer(rootConnectionString, e => e.MigrationsAssembly("Migrators.MSSQL")));
                break;

            default:
                throw new Exception($"DB Provider {dbProvider} is not supported.");
        }

        services.SetupDatabases<TA>(databaseSettings, moduleSettings);
        return services;
    }

    private static IServiceCollection SetupDatabases<TA>(this IServiceCollection services, DatabaseSettings options, ModuleManagementSettings moduleSettings)
    where TA : ApplicationDbContext
    {
        var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TA>();
        dbContext.Database.SetConnectionString(options.ConnectionString);
        switch (options.DBProvider.ToLower())
        {
            case "mssql":
                services.AddDbContext<TA>(m => m.UseSqlServer(e => e.MigrationsAssembly("Migrators.MSSQL")));
                break;
        }

        if (dbContext.Database.CanConnect())
        {
            try
            {
                if (moduleSettings.ModuleManagements != null)
                {
                    foreach (var item in moduleSettings.ModuleManagements)
                    {
                        SeedRootModules(dbContext, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return services;
    }

    private static T GetOptions<T>(this IServiceCollection services, string sectionName)
    where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);

        return options;
    }

    private static void SeedRootModules<T>(T dbContext, ModuleManagement options)
    where T : ApplicationDbContext
    {
        if (dbContext.Modules.Any(t => t.Tenant == options.Tenant))
        {
            dbContext.Modules.RemoveRange(dbContext.Modules.Where(t => t.Tenant == options.Tenant));
        }

        foreach (var module in options.Modules)
        {
            var rootModulle = new Module(module.Name, JsonConvert.SerializeObject(module.Permissions), options.Tenant, module.IsActive);
            dbContext.Modules.Add(rootModulle);
        }

        dbContext.SaveChanges();
    }
}
