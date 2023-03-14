using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Identity;

public class UpdateProfileRequest : IMustBeValid
{
    public string FullName { get; set; }
    public bool Status { get; set; }
    public List<string> IpAddresses { get; set; }
    public string AdminGroupId { get; set; }
    public FileUploadRequest Image { get; set; }
    public string BrandId { get; set; }
    public string ParentID { get; set; }
    public int RecordsToDisplay { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }

}

public class UpdateEmailRequest : IMustBeValid
{
    public string Email { get; set; }
    public string Password { get; set; }
}