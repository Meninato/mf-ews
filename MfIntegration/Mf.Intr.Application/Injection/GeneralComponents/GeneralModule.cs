using Autofac;
using Mf.Intr.Application.Schedulers;
using Mf.Intr.Core.Schedulers;
using Mf.Intr.Application.Services;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.DataAccess;
using Mf.Intr.Core.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mf.Intr.Core.Options;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Application.Injection.IntegrationContext;

public class GeneralModule : Module
{
    private readonly string _currentDirPath;

    public GeneralModule(string currentDirPath)
    { 
        _currentDirPath = currentDirPath; 
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<IntrDbContext>();
        builder.RegisterType<IntrDbContext>().WithParameter("currentDirPath", _currentDirPath).AsSelf();
       
        builder.RegisterType<MsSqlServerDbContext>().As<IMsSqlServerDbContext>();
        builder.Register((ctx, p) =>
        {
            var assemblyScan = ctx.Resolve<IAssemblyScanService>();
            var hanaDbContextType = assemblyScan.GetHanaPluginType(PluginDefaults.HANA_PLUGIN_CLASSNAME);

            var hanaDbContext = (IHanaDbContext)Activator.CreateInstance(hanaDbContextType, new object[]
            {
                ctx.Resolve<ILoggerFactory>(),
                ctx.Resolve<IOptions<AppOptions>>()
            })!;

            return hanaDbContext;

        }).As<IHanaDbContext>();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

        builder.RegisterType<EncryptorService>().As<IEncryptorService>();
        builder.RegisterType<AssemblyScanService>().As<IAssemblyScanService>().SingleInstance();
        builder.RegisterType<ManageablePoolService>().As<IManageablePoolService>().SingleInstance();

        builder.RegisterType<CompanyEventScheduler>().Keyed<IIntrScheduler>(SchedulerType.Company)
            .SingleInstance();

        builder.RegisterType<FileEventScheduler>().Keyed<IIntrScheduler>(SchedulerType.File)
            .SingleInstance();
    }
}
