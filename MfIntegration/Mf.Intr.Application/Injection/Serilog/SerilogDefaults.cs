using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Formatting.Compact;
using Serilog.Templates;
using Serilog.Core;
using System.Collections;
using Mf.Intr.Application.Helpers.Enrichers;
using Mf.Intr.Core.Options;
using Serilog.Filters;

namespace Mf.Intr.Application.Injection.Serilog;
internal class SerilogDefaults
{
    public const string DefaultLogTemplate
        = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

    public const string DefaultJsonFileLogTemplate
        = "{ {timestamp:@t, level:@l, rawMessage:@mt, message:@m, exception:@x, ..@p} }\n";

    public static readonly Dictionary<LogOutputTypes, string> SerilogOutputTypes = new Dictionary<LogOutputTypes, string>()
    {
        { LogOutputTypes.Text, DefaultLogTemplate },
        { LogOutputTypes.Json, DefaultJsonFileLogTemplate }
    };

    public static LoggerConfiguration DefaultLoggerConfiguration(string logPath, LogEventLevel logLevel, LogOutputTypes outputType)
    {
        var outputTemplate = SerilogOutputTypes[outputType];
        var loggerConfiguration = DefaultLoggerConfiguration(logLevel);

        if(outputType == LogOutputTypes.Text)
        {
            logPath = logPath + ".txt";
            loggerConfiguration.WriteTo.File(
                logPath,
                outputTemplate: outputTemplate,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 1024 * 1024 * 5,
                rollOnFileSizeLimit: true);
        }
        else if(outputType == LogOutputTypes.Json)
        {
            logPath = logPath + ".json";
            loggerConfiguration.WriteTo.File(
                formatter: new ExpressionTemplate(outputTemplate),
                logPath,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 1024 * 1024 * 5,
                rollOnFileSizeLimit: true);
        }

        return loggerConfiguration;
    }

    private static LoggerConfiguration DefaultLoggerConfiguration(LogEventLevel logLevel)
        => new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.With(new ExceptionEnricher())
            .MinimumLevel.Is(logLevel)
            .Filter.ByExcluding("StartsWith(SourceContext, 'Microsoft.EntityFrameworkCore') and SourceContext <> 'Microsoft.EntityFrameworkCore.Database.Command' ")
            .WriteTo.Console(outputTemplate: DefaultLogTemplate, theme: AnsiConsoleTheme.Literate);
}
