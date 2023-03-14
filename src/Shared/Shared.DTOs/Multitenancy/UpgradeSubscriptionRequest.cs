namespace MyReliableSite.Shared.DTOs.Multitenancy;

public class UpgradeSubscriptionRequest
{
    public string Tenant { get; set; }
    public DateTime ExtendedExpiryDate { get; set; }
}