using Autofac;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Schedulers;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Helpers.Extensions;

using Mf.Intr.Application.Helpers.NamedParameters;
using Mf.Intr.Core.Db.Entities;

namespace Mf.Intr.Application.Jobs;

public class CompanyEventJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
       ILogger? logger = Startup.Container.Resolve<ILogger<CompanyEventJob>>();

        try
        {
            await CallExecute(context, logger);
        }
        catch(Autofac.Core.DependencyResolutionException sourceEx)
        {
            var integrationEx = sourceEx.GetInnerExceptions().Where(e => e is IntegrationException).FirstOrDefault();
            var ex = integrationEx ?? sourceEx;
            logger.LogError(integrationEx ?? ex, "Failed at dependency resolution. {errorMsg}", ex.Message);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Something went wrong... {errorMsg}", ex.Message);
        }
        finally
        {
            logger = null;
        }
    }

    private async Task CallExecute(IJobExecutionContext context, ILogger logger)
    {
        var companyEvent = GetCompanyEventFromJobDataMap(context);

        //Take advantage of the autofac container
        using (var scope = Startup.Container.BeginLifetimeScope())
        {
            var manageablePool = scope.Resolve<IManageablePoolService>();

            logger.LogInformation("Job [{jobKey}] identified by [{jobName}] is running now", companyEvent.JobKey, companyEvent.EventGenerator.Name);

            IManageable manager = scope.Resolve<ICompanyManageable>(
                new NamedParameter(AppDefaults.NAMED_PARAMETER_COMPANYMANAGER, GetCompanyManagerNamedParameter(companyEvent)));

            manager = manageablePool.GetFromCache(manager, SchedulerType.Company, companyEvent.ID);

            logger.LogInformation("Manager [{key}:{name}] is ready and it's gonna work.", manager.Key, manager.Name);

            manager.StartWork();
        }

        await Task.CompletedTask;
    }

    private CompanyManagerNamedParameter GetCompanyManagerNamedParameter(CompanyEventEntity companyEvent)
    {
        return new CompanyManagerNamedParameter(
            companyEvent.EventGenerator.Manager,
            companyEvent.EventGeneratorID,
            companyEvent.SkipIfBusyStrategy,
            companyEvent.Connection,
            companyEvent.Query);
    }

    private CompanyEventEntity GetCompanyEventFromJobDataMap(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        return (CompanyEventEntity)dataMap[AppDefaults.JOB_DATA_COMPANYEVENT];
    }
}
