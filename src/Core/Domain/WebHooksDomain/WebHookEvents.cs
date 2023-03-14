namespace MyReliableSite.Domain.WebHooksDomain;

public class WebhookEvent
{
    /// <summary>
    /// Webhook unique name.
    /// </summary>
    public string WebhookName { get; set; }

    /// <summary>
    /// Webhook data as JSON string.
    /// </summary>
    public string Data { get; set; }
}

/// <summary>
/// Represents source hook trigger group.
/// </summary>
public enum HookEventType
{
    // *********************************************************************
    // Take this as example you can implement any custom event source
    // *********************************************************************

    /// <summary>
    /// Hook - created, hook removed, hook updated, hook enabled/disabled.
    /// </summary>
    hook,

    /// <summary>
    /// File - create, delete, revision.
    /// </summary>
    file,

    /// <summary>
    /// Note - create, update, deleted.
    /// </summary>
    note,

    /// <summary>
    /// Project - created, add/remove user, archived.
    /// </summary>
    project,

    /// <summary>
    /// Milestone - created, deleted, closed, re-opened.
    /// </summary>
    milestone

}