namespace MyReliableSite.Shared.DTOs.ManageModule;

public class UpdateSubUserModuleListRequest : IMustBeValid
{
    public List<CreateSubUserModuleManagementRequest> SubUserModules { get; set; }
}