using Autofac;
using Autofac.Features.Indexed;
using Mf.Intr.Application.Injection.Serilog;
using Mf.Intr.Core.Schedulers;
using Mf.Intr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Application;

public class IntegrationApp : IIntrApp
{
    public IIntrScheduler CompanyScheduler { get; private set; }
    public IIntrScheduler FileScheduler { get; private set; }
    public ILogger Logger { get; private set; }

    public IntegrationApp()
    {
        new Startup();
        
        CompanyScheduler = Startup.Container.ResolveKeyed<IIntrScheduler>(SchedulerType.Company);
        FileScheduler = Startup.Container.ResolveKeyed<IIntrScheduler>(SchedulerType.File);
        Logger = Startup.Container.Resolve<ILogger<IntegrationApp>>();
    }
}
