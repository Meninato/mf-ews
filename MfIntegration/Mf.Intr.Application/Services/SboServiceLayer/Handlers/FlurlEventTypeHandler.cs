using Flurl.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Handlers;

public abstract class FlurlEventTypeHandler : FlurlEventHandler
{
    protected readonly ILogger _logger;

    protected abstract FlurlEventType EventType { get; }

    public FlurlEventTypeHandler(ILogger logger)
    {
        _logger = logger;
    }

    protected virtual string GetRequestInfo(HttpRequestMessage httpRequest)
    {
        return string.Format("Service Layer is calling {0} event for {1} on {2}.",
            EventType,
            httpRequest.Method,
            httpRequest.RequestUri
        );
    }
}
