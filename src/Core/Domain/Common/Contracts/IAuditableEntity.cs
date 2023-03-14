namespace MyReliableSite.Domain.Common.Contracts;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public Guid? AdminAsClient { get; set; }
}