using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Domain.Products.Events;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Application.Products.EventHandlers;

public class ProductUpdatedEventHandler : INotificationHandler<EventNotification<ProductUpdatedEvent>>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;
    private readonly IScriptingService _scriptingService;
    private readonly IJobService _jobService;

    public ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger, IScriptingService scriptingService, IJobService jobService)
    {
        _logger = logger;
        _scriptingService = scriptingService;
        _jobService = jobService;
    }

    public async Task Handle(EventNotification<ProductUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);

        if (string.IsNullOrEmpty(notification.DomainEvent.Product.ModuleName)) return;

        HookType? hookType = null;
        switch (notification.DomainEvent.Product.Status)
        {
            case Shared.DTOs.Products.ProductStatus.Pending:
                break;
            case Shared.DTOs.Products.ProductStatus.Active:
                break;
            case Shared.DTOs.Products.ProductStatus.Cancelled:
                hookType = HookType.Cancel;
                break;
            case Shared.DTOs.Products.ProductStatus.Suspended:
                hookType = HookType.Suspend;
                break;
        }

        if (hookType.HasValue)
        {
            var request = new RunHooksRequest() { HookType = hookType.Value, ProductId = notification.DomainEvent.Product.Id };
            await _scriptingService.RunHooksAsync(request);
        }
    }
}
