using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain;

public class WebHookHeader : AuditableEntity, IMustHaveTenant
{

    /// <summary>
    /// Linked Webhook Id.
    /// </summary>
    public Guid WebHookID { get; set; }

    /// <summary>
    /// Linked Webhook.
    /// </summary>
    public WebHook WebHook { get; set; }

    /// <summary>
    /// Header Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Header content.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Header created time.
    /// </summary>
    public DateTime CreatedTimestamp { get; set; }
    public string Tenant { get; set; }
}
