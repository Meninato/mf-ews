using Autofac;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Application.Helpers;
using Autofac.Core;
using Mf.Intr.Application.Helpers.NamedParameters;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Helpers;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

namespace Mf.Intr.Application.Injection.Worker;

public class WorkerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(CreateFileWorkable).As<IFileWorkable>();
        builder.Register(CreateCompanyWorkable).As<ICompanyWorkable>();
        builder.Register(CreateSharedWorkable).As<ISharedWorkable>();
        //builder.Register(CreateWorkable).As<IWorkable>();
    }

    private IFileWorkable CreateFileWorkable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var workerParameter = p.Named<FileWorkerNamedParameter>(AppDefaults.NAMED_PARAMETER_FILEWORKER);
        var workableType = ctx.Resolve<IAssemblyScanService>().GetFileWorkableType(workerParameter.Worker.ClassName);

        IFileWorkable workableInstance = (IFileWorkable)Activator.CreateInstance(workableType, new object[]
        {
            CreateServiceBox(ctx, workableType)
        })!;

        return workableInstance;
    }

    private ICompanyWorkable CreateCompanyWorkable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var workerParameter = p.Named<CompanyWorkerNamedParameter>(AppDefaults.NAMED_PARAMETER_COMPANYWORKER);
        var workableType = ctx.Resolve<IAssemblyScanService>().GetCompanyWorkableType(workerParameter.Worker.ClassName);

        ICompanyWorkable workableInstance = (ICompanyWorkable)Activator.CreateInstance(workableType, new object[]
        {
            CreateServiceBox(ctx, workableType)
        })!;

        return workableInstance;
    }

    private ISharedWorkable CreateSharedWorkable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var workerParameter = p.Named<SharedWorkerNamedParameter>(AppDefaults.NAMED_PARAMETER_SHAREDWORKER);
        var workableType = ctx.Resolve<IAssemblyScanService>().GetSharedWorkableType(workerParameter.Worker.ClassName);

        ISharedWorkable workableInstance = (ISharedWorkable)Activator.CreateInstance(workableType, new object[]
        {
            CreateServiceBox(ctx, workableType)
        })!;

        return workableInstance;
    }

    [Obsolete("Use a specific worker instead.")]
    private IWorkable CreateWorkable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var workerParameter = p.Named<WorkerNamedParameter>(AppDefaults.NAMED_PARAMETER_WORKER);
        var workableType = ctx.Resolve<IAssemblyScanService>().GetWorkableType(workerParameter.Worker.ClassName);

        IWorkable workableInstance = (IWorkable)Activator.CreateInstance(workableType, new object[]
        {
            CreateServiceBox(ctx, workableType)
        })!;

        return workableInstance;
    }

    public static void SetRequiredPropertiesAndParameters(IFileWorkable workableInstance,
    bool isFirstWorker, WorkerEntity workerEntity, DirectoryInfo directory, FileInfo file)
    {
        SetRequiredPropertiesAndParameters(workableInstance, isFirstWorker, workerEntity);

        var fileWorkerInitParameters = new object?[2] {
            directory,
            file
        };

        var methodInfo = ReflectionUtil.GetMethods(workableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_WORKER_FILE)
            .FirstOrDefault();

        if (methodInfo != null)
        {
            methodInfo.Invoke(workableInstance, fileWorkerInitParameters);
        }
    }

    public static void SetRequiredPropertiesAndParameters(ICompanyWorkable workableInstance, 
        bool isFirstWorker, WorkerEntity workerEntity, string? query, ConnectionEntity connection)
    {
        SetRequiredPropertiesAndParameters(workableInstance, isFirstWorker, workerEntity);
        
        var companyWorkerInitParameters = new object?[2] {
            query,
            connection
        };

        var methodInfo = ReflectionUtil.GetMethods(workableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_WORKER_COMPANY)
            .FirstOrDefault();

        if (methodInfo != null)
        {
            methodInfo.Invoke(workableInstance, companyWorkerInitParameters);
        }
    }

    public static void SetRequiredPropertiesAndParameters(ISharedWorkable workableInstance, 
        bool isFirstWorker, WorkerEntity workerEntity)

    {
        SetRequiredPropertiesAndParameters(workableInstance, isFirstWorker, workerEntity);
    }

    public static void SetRequiredPropertiesAndParameters(IWorkable workableInstance, bool isFirstWorker, WorkerEntity workerEntity)
    {
        var workerInitParameters = new object?[5] {
            workerEntity.ID,
            workerEntity.Name,
            workerEntity.PassWorker,
            workerEntity.FailWorker,
            isFirstWorker
        };

        var methodInfo = ReflectionUtil.GetMethods(workableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_WORKER)
            .FirstOrDefault();

        if(methodInfo != null)
        {
            methodInfo.Invoke(workableInstance, workerInitParameters);
        }

        //set all parameters if any
        SetParameters(workerEntity, workableInstance);
    }

    private static void SetParameters(WorkerEntity workerEntity, IWorkable workableInstance)
    {
        if (workerEntity.Parameters != null && workerEntity.Parameters.Count > 0)
        {
            var wrongParameter = workerEntity.Parameters.FirstOrDefault(p => string.IsNullOrEmpty(p.Name));
            if (wrongParameter != null)
            {
                throw new IntegrationException($"Were identified parameters name with null value when setting parameters for Worker");
            }

            Type workableType = workableInstance.GetType();
            foreach (var parameter in workerEntity.Parameters)
            {
                try
                {
                    workableType.GetProperty(parameter.Name)?.SetValue(workableInstance,
                        ParameterCaster.CastParameterValue(parameter.Type, parameter.Value));
                }
                catch (Exception ex)
                {
                    throw new ParameterCastException($"Worker [{workableInstance.Key}:{workableInstance.Name}]. Error while parsing parameter name {parameter.Name} for Worker. {ex.Message}",
                        parameter.Name!, ex);
                }
            }
        }
    }

    private IWorkerServiceBox CreateServiceBox(IComponentContext ctx, Type workableType)
    {
        var serviceBox = new WorkerServiceBox();
        serviceBox.Add((ILogger)ctx.Resolve(typeof(ILogger<>).MakeGenericType(workableType)));
        serviceBox.Add(ctx.Resolve<IEncryptorService>());
        serviceBox.Add(ctx.Resolve<IConfigurationService>());
        serviceBox.Add(ctx.Resolve<IUnitOfWork>());
        serviceBox.Add(ctx.Resolve<ISboServiceLayerPoolService>());

        return serviceBox;
    }
}
