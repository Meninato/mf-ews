using Autofac;
using Mf.Intr.Application.Injection.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.Manager;

public static class ManagerContainerBuilderExtension
{
    public static ContainerBuilder RegisterManager(this ContainerBuilder builder)
    {
        builder.RegisterModule<ManagerModule>();

        return builder;
    }
}
