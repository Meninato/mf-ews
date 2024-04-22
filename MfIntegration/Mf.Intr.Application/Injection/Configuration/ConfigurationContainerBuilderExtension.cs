using Autofac;
using Mf.Intr.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.Configuration;

public static class ConfigurationContainerBuilderExtension
{
    public static ContainerBuilder RegisterConfiguration(this ContainerBuilder builder, string basePath)
    {
        var envValues = new string[] { AppDefaults.ENVIRONMENT_VALUE_DEV, AppDefaults.ENVIRONMENT_VALUE_PROD };
        var environmentName = Environment.GetEnvironmentVariable(AppDefaults.ENVIRONMENT_VARIABLE) ?? AppDefaults.ENVIRONMENT_VALUE_DEV;

        if(envValues.Contains(environmentName) == false)
        {
            throw new IntegrationException($"Not found the value {environmentName} environment variable and/or wrong value");
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .Build();

        builder.RegisterModule(new ConfigModule(config));

        return builder;
    }
}
