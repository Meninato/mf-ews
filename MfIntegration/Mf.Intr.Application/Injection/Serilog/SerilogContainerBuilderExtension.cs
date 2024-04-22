using Autofac;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Options;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.Serilog;
public static class SerilogContainerBuilderExtension
{
    public static ContainerBuilder RegisterSerilog(this ContainerBuilder builder, string logPath,
            LogEventLevel logEventLevel = LogEventLevel.Debug)
    {
        builder.RegisterModule<SerilogModule>();

        builder.RegisterBuildCallback(ctx =>
        {
            var appOptions = ctx.Resolve<IOptions<AppOptions>>().Value;
            var loggerConfiguration = SerilogDefaults.DefaultLoggerConfiguration(logPath, logEventLevel, appOptions.LogOutputType);

            Log.Logger = loggerConfiguration
                .CreateLogger();
        });

        return builder;
    }
}
