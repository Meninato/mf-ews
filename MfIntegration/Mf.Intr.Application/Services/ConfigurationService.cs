using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services;
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration config)
    {
        _configuration = config;
    }

    public T Get<T>(string section)
    {
        return _configuration.GetSection(section).Get<T>();
    }

    public AppOptions GetAppOptions()
    {
        return _configuration.GetSection(AppDefaults.APP_OPTIONS).Get<AppOptions>();
    }
}
