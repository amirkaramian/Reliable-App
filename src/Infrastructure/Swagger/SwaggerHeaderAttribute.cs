namespace MyReliableSite.Infrastructure.Swagger;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerHeaderAttribute : Attribute
{
    public string HeaderName { get; set; }
    public string Description { get; set; }
    public string ModuleName { get; set; }
    public string ModuleAction { get; set; }
    public string DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public bool IsClient { get; set; }
    public bool IsModuleCheckReq { get; set; }

    public SwaggerHeaderAttribute(string headerName, string moduleName, string moduleAction, string description = null, string defaultValue = null, bool isRequired = false, bool isClient = false, bool isModuleCheckReq = true)
    {
        HeaderName = headerName;
        ModuleName = moduleName;
        ModuleAction = moduleAction;
        Description = description;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
        IsClient = isClient;
        IsModuleCheckReq = isModuleCheckReq;
    }
}
