using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.WebHooksDTO;

/// <summary>
/// Defines event object.
/// </summary>
public class WebhookDomainEvent
{
    public long ID { get; set; }

#nullable enable
    public Guid? ActorID { get; set; }
#nullable disable

    public DateTime TimeStamp { get; set; }

    public EventType EventType { get; set; }
}

public enum EventType
{
    WebHook,
    System,
    Project,
}

/// <summary>
/// WebHookCreated.
/// </summary>
public class WebHookCreated : WebhookDomainEvent
{

    public WebHookCreated()
    {

    }

    public long WebHookId { get; set; }

    // Add any custom props hire...
}

/// <summary>
/// WebHookRemoved.
/// </summary>
public class WebHookRemoved : WebhookDomainEvent
{

    public WebHookRemoved()
    {

    }

    public long WebHookId { get; set; }

    // Add any custom props hire...

}

/// <summary>
/// WebHookUpdated.
/// </summary>
public class WebHookUpdated : WebhookDomainEvent
{

    public WebHookUpdated()
    {

    }

    public long WebHookId { get; set; }

    // Add any custom props here...

}