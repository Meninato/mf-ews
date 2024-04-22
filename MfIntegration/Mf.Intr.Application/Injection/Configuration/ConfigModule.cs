using Autofac;
using Mf.Intr.Application.Services;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.Configuration;

public class ConfigModule : Module
{
    private IConfigurationRoot _configurationRoot;

    public ConfigModule(IConfigurationRoot configurationRoot)
    {
        _configurationRoot = configurationRoot;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(_configurationRoot).As<IConfiguration>().SingleInstance();
        builder.RegisterGeneric(typeof(OptionsWrapper<>)).As(typeof(IOptions<>)).SingleInstance();
        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            return Options.Create(config.GetSection(AppDefaults.APP_OPTIONS).Get<AppOptions>());
        });
        builder.RegisterType<ConfigurationService>().As<IConfigurationService>();
    }
}
