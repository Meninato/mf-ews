using Autofac;
using Mf.Intr.Application.Injection.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.SboServiceLayer;

public static class SboServiceLayerContainerBuilderExtension
{
    public static ContainerBuilder RegisterSboServiceLayer(this ContainerBuilder builder)
    {
        builder.RegisterModule<SboServiceLayerModule>();

        return builder;
    }
}
