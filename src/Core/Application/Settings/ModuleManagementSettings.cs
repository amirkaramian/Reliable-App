namespace MyReliableSite.Application.Settings;

public class ModuleManagementSettings
{
    public List<ModuleManagement> ModuleManagements { get; set; }
}

public class ModuleManagement
{
    public List<ModuleAppLevel> Modules { get; set; }
    public string Tenant { get; set; }
}

public class ModuleAppLevel
{
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public Dictionary<string, bool> Permissions { get; set; }
}
