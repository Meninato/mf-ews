using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.AssemblyScan;

public static class AssemblyScanContainerBuilderExtension
{
    public static ContainerBuilder RegisterAssemblyScan(this ContainerBuilder builder, string assemblyDirPath, string pluginDirPath)
    {
        List<string> pluginAssemblies = new List<string>()
        {
            Path.Combine(pluginDirPath, AppDefaults.PLUGIN_HANA_FOLDER, PluginDefaults.HANA_PLUGIN_ASSEMBLY)
        };

        builder.RegisterModule(new AssemblyScanModule(assemblyDirPath, pluginAssemblies));
        return builder;
    }
}
