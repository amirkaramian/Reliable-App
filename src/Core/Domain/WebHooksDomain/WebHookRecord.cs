using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain;

// DTO
public class WebHookRecord : AuditableEntity, IMustHaveTenant
{
    public WebHookRecord()
    {
    }

    public WebHookRecord(
        Guid webHookID, HookEventType hookType, RecordResult result, int statusCode, string responseBody, string requestBody, string requestHeaders, string exception, DateTime timeStamp)
    {
        HookType = hookType;
        Result = result;
        StatusCode = statusCode;
        RequestBody = requestBody;
        ResponseBody = responseBody;
        RequestHeaders = requestHeaders;
        Exception = exception;
        Timestamp = timeStamp;
        WebHookID = webHookID;
    }

    public WebHookRecord Update(Guid webHookID, HookEventType hookType, RecordResult result, int statusCode, string responseBody, string requestBody, string requestHeaders, string exception, DateTime timeStamp)
    {
        if (webHookID != WebHookID) { WebHookID = webHookID; }
        if (hookType != HookType) { HookType = hookType; }
        if (result != Result) { Result = result; }
        if (responseBody != null && !ResponseBody.NullToString().Equals(responseBody)) ResponseBody = responseBody;
        if (requestBody != null && !RequestBody.NullToString().Equals(requestBody)) RequestBody = requestBody;
        if (requestHeaders != null && !RequestHeaders.NullToString().Equals(requestHeaders)) RequestHeaders = requestHeaders;
        if (exception != null && !Exception.NullToString().Equals(exception)) Exception = exception;
        if (statusCode != StatusCode) { StatusCode = statusCode; }
        if (timeStamp != Timestamp) { Timestamp = timeStamp; }
        return this;
    }

    /// <summary>
    /// Linked Webhook Id.
    /// </summary>
    public Guid WebHookID { get; set; }

    /// <summary>
    /// Linked Webhook.
    /// </summary>
    public WebHook WebHook { get; set; }

    /// <summary>
    /// WebHookType.
    /// </summary>
    public HookEventType HookType { get; set; }

    /// <summary>
    /// Hook result enum.
    /// </summary>
    public RecordResult Result { get; set; }

    /// <summary>
    /// Response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Response json.
    /// </summary>
    public string ResponseBody { get; set; }

    /// <summary>
    /// Request json.
    /// </summary>
    public string RequestBody { get; set; }

    /// <summary>
    /// Request Headers json.
    /// </summary>
    public string RequestHeaders { get; set; }

    /// <summary>
    /// Exception.
    /// </summary>
    public string Exception { get; set; }

    /// <summary>
    /// Hook Call Timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }
    public string Tenant { get; set; }
}

public enum RecordResult
{
    undefined = 0,
    ok,
    parameter_error,
    http_error,
    dataQueryError
}
