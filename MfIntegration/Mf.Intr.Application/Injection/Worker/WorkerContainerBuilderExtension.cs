using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Injection.Worker;

public static class WorkerContainerBuilderExtension
{
    public static ContainerBuilder RegisterWorker(this ContainerBuilder builder)
    {
        builder.RegisterModule<WorkerModule>();

        return builder;
    }
}
