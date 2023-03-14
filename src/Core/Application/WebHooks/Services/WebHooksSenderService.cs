using MyReliableSite.Application.WebHooks.Interfaces;
using System.Text;
using MyReliableSite.Domain.WebHooksDomain;
using MyReliableSite.Domain.ManageModule;
using Newtonsoft.Json;
using MyReliableSite.Application.Common.Interfaces;
using System.Net;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace MyReliableSite.Application.WebHooks.Services;

public class WebHooksSenderService : IWebHooksSenderService
{
    private readonly IRepositoryAsync _repository;

    public WebHooksSenderService(IRepositoryAsync repository)
    {
        _repository = repository;
    }

    private async Task UpdateWebHookToInactive(string moduleName)
    {
        var module = await _repository.FirstByConditionAsync<Module>(m => m.Name == moduleName);
        if (module != null)
        {
            module.IsActive = false;
        }

        await _repository.SaveChangesAsync();
    }

    private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
            2,
            retryAttempt => TimeSpan.FromSeconds(30),
            onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
            {
                int retryCount = retryAttempt;
                if (retryCount >= 2)
                {
                    object moduleName = context["moduleName"];
                    await UpdateWebHookToInactive(moduleName.ToString());
                }
            })
            .WrapAsync(Policy.TimeoutAsync(60, TimeoutStrategy.Optimistic));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    public async Task sendWebHook<T>(T model, string moduleName)
    {
        var policy = GetRetryPolicy();

        var circuitBreaker = GetCircuitBreakerPolicy();
        HttpClient httpClient = new HttpClient();

        var module = await _repository.FirstByConditionAsync<Module>(m => m.Name == moduleName);
        if (module != null)
        {
            var webhook = await _repository.FirstByConditionAsync<WebHook>(m => m.ModuleId == module.Id.ToString());
            if (webhook != null && webhook.IsActive)
            {
                var postContent = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    System.Net.Mime.MediaTypeNames.Application.Json);

                await policy.ExecuteAsync(
                     async (Context, CancellationToken) => await httpClient.PostAsync(webhook.WebHookUrl, postContent),
                     new Dictionary<string, object>() { { "moduleName", moduleName } },
                     CancellationToken.None);

            }
        }

    }

    private static async Task<HttpResponseMessage> createPostRequest(HttpClient httpClient, WebHook webhook, StringContent postContent)
    {
        return await httpClient.PostAsync(webhook.WebHookUrl, postContent);
    }
}

