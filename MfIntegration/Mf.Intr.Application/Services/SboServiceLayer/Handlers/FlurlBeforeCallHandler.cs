using Flurl.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Handlers;

public class FlurlBeforeCallHandler : FlurlEventTypeHandler
{
    protected override FlurlEventType EventType => FlurlEventType.BeforeCall;

    public FlurlBeforeCallHandler(ILogger<FlurlBeforeCallHandler> logger) : base(logger) { }

    public override async Task HandleAsync(FlurlEventType eventType, FlurlCall call)
    {
        _logger.LogInformation("{event}", GetRequestInfo(call.HttpRequestMessage));

        await Task.CompletedTask;
    }
}