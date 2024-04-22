using Autofac;
using Mf.Intr.Application.Injection.IntegrationContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.GeneralComponents;

public static class GeneralContainerBuilderExtension
{
    public static ContainerBuilder RegisterGeneralComponents(this ContainerBuilder builder, string currentDirPath)
    {
        builder.RegisterModule(new GeneralModule(currentDirPath));
        return builder;
    }
}
