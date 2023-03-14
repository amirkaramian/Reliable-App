using System.Text;
using System.Text.Json;
using MyReliableSite.Application.WebHooks.Interfaces;
using MyReliableSite.Shared.DTOs.WebHooks;
using MediatR;

namespace MyReliableSite.Application.WebHooks.EventHandlers;
/*
public class ProcessWebHook : IRequest
{
    public Guid HookId { get; set; }
    public dynamic Event { get; set; }
    public Shared.DTOs.WebHooks.HookEventType EventType { get; set; }
}

/// <summary>
/// Command handler for <c>ProcessWebHook</c>.
/// </summary>
public class ProcessWebHookHandler : IRequestHandler<ProcessWebHook, Unit>
{

    /// <summary>
    /// Injected <c>IMediator</c>.
    /// </summary>
    private readonly IWebHooksService _webHooksService;
    private readonly IWebHookRecordService _webHookRecordService;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Main Constructor.
    /// </summary>
    public ProcessWebHookHandler(IWebHooksService webHooksService, IWebHookRecordService webHookRecordService, HttpClient httpClient)
    {
        _webHooksService = webHooksService;
        _webHookRecordService = webHookRecordService;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Command handler for  <c>ProcessWebHook</c>.
    /// </summary>
    public async Task<Unit> Handle(ProcessWebHook request, CancellationToken cancellationToken)
    {

        CreateWebHookRecordRequest record = new CreateWebHookRecordRequest()
        {
            WebHookId = request.HookId,
            HookType = request.EventType,
            Timestamp = DateTime.Now
        };

        if (request == null)
        {
            record.Result = Shared.DTOs.WebHooks.RecordResult.parameter_error;
        }

        try
        {

            WebHooksDetailsDto hook = null;

            try
            {
                var data = await _webHooksService.GetWebHooksDetailsAsync(request.HookId);
                if (data != null)
                {
                    hook = data.Data;
                }

            }
            catch (Exception ex)
            {
                record.Result = Shared.DTOs.WebHooks.RecordResult.dataQueryError;
                record.Exception = ex.ToString();

                return Unit.Value;
            }

            if (hook != null)
            {

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true,
                };

                var serialised_request_body = new StringContent(
                      JsonSerializer.Serialize<dynamic>(request.Event, options),
                      Encoding.UTF8,
                      "application/json");

                /* Set Headers
                _httpClient.DefaultRequestHeaders.Add("X-Trouble-Delivery", record.WebHookId.ToString());

                if (!string.IsNullOrWhiteSpace(hook.Secret))
                {
                    _httpClient.DefaultRequestHeaders.Add("X-Trouble-Secret", hook.Secret);
                }

                _httpClient.DefaultRequestHeaders.Add("X-Trouble-Event", request.EventType.ToString().ToLowerInvariant());

                record.RequestBody = await serialised_request_body.ReadAsStringAsync(cancellationToken);

                var serialized_headers = new StringContent(
                                 JsonSerializer.Serialize(_httpClient.DefaultRequestHeaders.ToList(), options),
                                 Encoding.UTF8,
                                 "application/json");

                record.RequestHeaders = await serialized_headers.ReadAsStringAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(hook.WebHookUrl))
                {
                    try
                    {
                        using var httpResponse = await _httpClient.PostAsync(hook.WebHookUrl, serialised_request_body);

                        if (httpResponse != null)
                        {
                            record.StatusCode = (int)httpResponse.StatusCode;

                            if (httpResponse.Content != null)
                            {
                                record.ResponseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                            }
                        }

                        record.Result = Shared.DTOs.WebHooks.RecordResult.ok;
                    }
                    catch (Exception ex)
                    {
                        record.Result = Shared.DTOs.WebHooks.RecordResult.http_error;
                        record.Exception = ex.ToString();
                    }
                }
                else
                {
                    record.Result = Shared.DTOs.WebHooks.RecordResult.parameter_error;
                }
            }
            else
            {
                record.Result = Shared.DTOs.WebHooks.RecordResult.parameter_error;
            }

        }
        finally
        {

            try
            {
                await _webHookRecordService.CreateWebHookRecordAsync(record);
            }
            catch { }

        }

        return Unit.Value;
    }
}*/