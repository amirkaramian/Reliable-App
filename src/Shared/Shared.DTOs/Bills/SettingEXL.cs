namespace MyReliableSite.Shared.DTOs.Billing;

public class SettingEXL
{
    public Guid Id { get; set; }
    public string BillNo { get; set; }
    public string UserId { get; set; }
    public Guid OrderId { get; set; }
    public string DueDate { get; set; }
    public decimal? VAT { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public decimal? SubTotal { get; set; }

}