namespace MyReliableSite.Application.Settings;

public class CorsSettings : IAppSettings
{
    public string Angular { get; set; }
    public string Blazor { get; set; }
    public bool EnableAnyOriginCors { get; set; }
}
