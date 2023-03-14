using MediatR;
using MyReliableSite.Application.WebHooks.Interfaces;
using MyReliableSite.Domain.WebHooksDomain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.WebHooks.EventHandlers;
/*
/// <summary>
/// Command for processing WebHook event.
/// </summary>
public class EnqueueRelatedWebHooks : IRequest
{
    public HookEventType EventType { get; set; }
    public object Event { get; set; }
}

/// <summary>
/// Command handler for <c>EnqueueRelatedWebHooks</c>.
/// </summary>
public class EnqueueRelatedWebHooksHandler : IRequestHandler<EnqueueRelatedWebHooks, Unit>
{

    /// <summary> Injected <c>IMediator</c> </summary>
    private readonly IMediator _mediator;

    private readonly IWebHooksService _webHooksService;

    /// <summary> Main Constructor </summary>
    public EnqueueRelatedWebHooksHandler(IWebHooksService webHooksService, IMediator mediator)
    {
        _webHooksService = webHooksService;
        _mediator = mediator;
    }

    /// <summary> Command handler for  <c>EnqueueRelatedWebHooks</c> </summary>
    public async Task<Unit> Handle(EnqueueRelatedWebHooks request, CancellationToken cancellationToken)
    {

        if (request == null || request.Event == null)
        {
            throw new ArgumentNullException();
        }


        List<WebHook> hooks = await _webHooksService.GetWebHooksDetailsAsync(request.HookId)
        .AsNoTracking()
        .Where(e => e.HookEvents.Contains(HookEventType.hook))
        .ToListAsync(cancellationToken);

        if (hooks != null)
        {
            foreach (var hook_item in hooks)
            {
                if (hook_item.IsActive && hook_item.ID > 0)
                {
                    try
                    {
                        _mediator.Enqueue(new ProcessWebHook()
                        {
                            HookId = hook_item.ID,
                            Event = request.Event,
                            EventType = request.EventType
                        });
                    }
                    catch { }
                }
            }
        }

        return Unit.Value;
    }
}
}
*/