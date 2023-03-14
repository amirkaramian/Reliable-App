using MediatR;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Domain.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Scripting.EventHandlers;

public class GlobalHandler : INotificationHandler<EventNotification<GlobalEvent>>
{
    private readonly IScriptingService _scriptingService;
    private readonly IJobService _jobService;

    public GlobalHandler(IScriptingService scriptingService, IJobService jobService)
    {
        _scriptingService = scriptingService;
        _jobService = jobService;
    }

    public async Task Handle(EventNotification<GlobalEvent> notification, CancellationToken cancellationToken)
    {
        string trigger = notification.DomainEvent.Data.GetType().Name;
        object data = notification.DomainEvent.Data;
        await _scriptingService.TriggerHooks(trigger, data);
    }
}
