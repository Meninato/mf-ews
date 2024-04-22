using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers.Enrichers;

/// <summary>
/// The default exception for the json format doesn't add the stack trace.
/// This enricher will add more information regarding the exception.
/// </summary>
public class ExceptionEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        //TODO: add the extension GetInnerExceptions in the serilog Enricher

        if (logEvent.Exception == null) return;

        var dataDictionary = new Dictionary<string, object?>
        {
            { "type", logEvent.Exception.GetType().ToString() },
            { "message", logEvent.Exception.Message },
            { "stackTrace", logEvent.Exception.StackTrace }
        };

        var property = propertyFactory.CreateProperty("exception", dataDictionary, destructureObjects: true);

        logEvent.AddOrUpdateProperty(property);
    }
}
