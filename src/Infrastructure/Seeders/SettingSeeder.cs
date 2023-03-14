using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using System.Reflection;

namespace MyReliableSite.Infrastructure.Seeders;

public class SettingSeeder : IDatabaseSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SettingSeeder> _logger;

    public SettingSeeder(ISerializerService serializerService, ILogger<SettingSeeder> logger, ApplicationDbContext db)
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
            if (!_db.Settings.Any())
            {
                _logger.LogInformation("Started to Seed Settings.");

                // Here you can use your own logic to populate the database.
                // As an example, I am using a JSON file to populate the database.
                string settingData = await File.ReadAllTextAsync(path + "/Seeders/settingSeed.json");
                var settings = _serializerService.Deserialize<List<Setting>>(settingData);

                if (settings != null)
                {
                    foreach (var setting in settings)
                    {
                        await _db.Settings.AddAsync(setting);
                    }
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded Settings.");
            }
        }).GetAwaiter().GetResult();
    }
}
