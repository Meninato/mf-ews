using Flurl.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Handlers;

public class FlurlOnRedirectHandler : FlurlEventTypeHandler
{
    protected override FlurlEventType EventType => FlurlEventType.OnRedirect;

    public FlurlOnRedirectHandler(ILogger<FlurlOnRedirectHandler> logger) : base(logger) { }

    public override async Task HandleAsync(FlurlEventType eventType, FlurlCall call)
    {
        _logger.LogInformation("{event}", GetRequestInfo(call.HttpRequestMessage));

        await Task.CompletedTask;
    }
}