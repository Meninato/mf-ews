using Autofac;
using Mf.Intr.Application.Services;
using Mf.Intr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.AssemblyScan;

public class AssemblyScanModule : Module
{
    private readonly string _assemblyDirPath;
    private readonly List<string> _assembliesPlugin;

    public AssemblyScanModule(string assemblyDirPath, List<string> assembliesPlugin)
    {
        _assemblyDirPath = assemblyDirPath;
        _assembliesPlugin = assembliesPlugin;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AssemblyScanService>()
            .As<IAssemblyScanService>()
            .SingleInstance();

        builder.RegisterBuildCallback(ctx =>
        {
            var assemblyScan = ctx.Resolve<IAssemblyScanService>();
            assemblyScan.Load(_assemblyDirPath);
            if(_assembliesPlugin != null && _assembliesPlugin.Count > 0)
            {
                _assembliesPlugin.ForEach((pluginAssembly) => assemblyScan.Add(pluginAssembly));
            }
        });
    }
}
