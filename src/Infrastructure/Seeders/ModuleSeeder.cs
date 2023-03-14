using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using Newtonsoft.Json;
using System.Reflection;

namespace MyReliableSite.Infrastructure.Seeders;

public class ModuleSeeder : IDatabaseSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ModuleSeeder> _logger;

    public ModuleSeeder(ISerializerService serializerService, ILogger<ModuleSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public void Initialize()
    {
        Task.Run(async () =>
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!_db.Modules.Any())
            {
                _logger.LogInformation("Started to Seed Module.");

                // Here you can use your own logic to populate the database.
                // As an example, I am using a JSON file to populate the database.
                string settingData = await File.ReadAllTextAsync(path + "/Seeders/moduleSeed.json");
                var modules = _serializerService.Deserialize<List<MyReliableSite.Domain.ManageModule.Module>>(settingData);

                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        await _db.Modules.AddAsync(module);
                    }

                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Seeded Modules.");
            }
            else
            {
                _logger.LogInformation("Started to Seed Module.");
                var modulesList = _db.Modules;

                string settingData = await File.ReadAllTextAsync(path + "/Seeders/moduleSeed.json");
                var modules = _serializerService.Deserialize<List<ModuleSeed>>(settingData);
                if (modules != null)
                {
                    bool hasChanges = false;
                    foreach (var module in modules.Where(m => m.HasChanges))
                    {
                        var thisModule = modulesList.FirstOrDefault(m => m.Tenant == module.Tenant && m.Name == module.Name);
                        if (thisModule != null)
                        {
                            thisModule.PermissionDetail = JsonConvert.SerializeObject(module.Permissions);
                            thisModule.IsActive = module.IsActive;
                        }
                        else
                        {
                            await _db.Modules.AddAsync(
                                new MyReliableSite.Domain.ManageModule.Module(module.Name, JsonConvert.SerializeObject(module.Permissions), module.Tenant, module.IsActive));
                        }

                        module.HasChanges = false;
                        hasChanges = true;
                    }

                    if (hasChanges)
                    {
                        string jsonString = JsonConvert.SerializeObject(modules);
                        File.WriteAllText(path + "/Seeders/moduleSeed.json", jsonString);
                    }

                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Seeded Modules.");
            }
        }).GetAwaiter().GetResult();
    }
}

internal class ModuleSeed
{

    public string Name { get; set; }
    public Dictionary<string, bool> Permissions { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public bool HasChanges { get; set; }
}