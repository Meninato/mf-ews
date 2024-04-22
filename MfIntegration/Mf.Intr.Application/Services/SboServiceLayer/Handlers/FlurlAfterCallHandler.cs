using Flurl.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Handlers;

public class FlurlAfterCallHandler : FlurlEventTypeHandler
{
    protected override FlurlEventType EventType => FlurlEventType.AfterCall;

    public FlurlAfterCallHandler(ILogger<FlurlAfterCallHandler> logger) : base(logger) { }

    public override async Task HandleAsync(FlurlEventType eventType, FlurlCall call)
    {
        var content = await call.HttpResponseMessage.Content.ReadAsStringAsync();
        _logger.LogInformation("{event} Status: {status}. Response: {content}",
            GetRequestInfo(call.HttpRequestMessage),
            call.Response.StatusCode,
            content);

        await Task.CompletedTask;
    }
}