namespace MyReliableSite.Shared.DTOs.ManageUserApiKey;

public class APIKeyPairEXL
{
    public Guid Id { get; set; }
    public string ApplicationKey { get; set; }
    public string UserIds { get; set; }
    public string SafeListIpAddresses { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public string Label { get; set; }

    // public string Roles { get; set; }
}
