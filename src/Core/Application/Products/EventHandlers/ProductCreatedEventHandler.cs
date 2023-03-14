using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Domain.Products.Events;

namespace MyReliableSite.Application.Products.EventHandlers;

public class ProductCreatedEventHandler : INotificationHandler<EventNotification<ProductCreatedEvent>>
{
    private const string _hookName = "ProductCreatedHook";
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly IScriptingService _scriptingService;
    private readonly IRepositoryAsync _repository;
    private readonly IJobService _jobService;
    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger, IScriptingService scriptingService, IRepositoryAsync repository, IJobService jobService)
    {
        _logger = logger;
        _scriptingService = scriptingService;
        _repository = repository;
        _jobService = jobService;
    }

    public Task Handle(EventNotification<ProductCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}