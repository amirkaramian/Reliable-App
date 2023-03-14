using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.WebHooks.Services;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Reflection;

namespace MyReliableSite.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IRequestValidator>();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        /*services.AddHttpClient<WebHooksSenderService>("PollyWaitAndRetry")
            .AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)

.Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                4,
                retryNumber => TimeSpan.FromSeconds(30),
                onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                if (retryAttempt => 120)
                {

                }
            })

    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.CircuitBreakerAsync(4, TimeSpan.FromMinutes(8)))
    .SetHandlerLifetime(TimeSpan.FromMinutes(60))
    .WrapAsync(Policy.TimeoutAsync(1)));*/
        return services;
    }
}