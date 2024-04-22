using Autofac;
using Mf.Intr.Core.Interfaces.Db;
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
using Mf.Intr.Core.Managers;
using Mf.Intr.Application.Helpers.NamedParameters;
using Mf.Intr.Application.Injection.Worker;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Helpers;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

namespace Mf.Intr.Application.Injection.Manager;

public class ManagerModule : Module
{ 
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(CreateFileManageable).As<IFileManageable>();
        builder.Register(CreateCompanyManageable).As<ICompanyManageable>();
        //builder.Register(CreateManageable).As<IManageable>();
    }

    private IFileManageable CreateFileManageable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var fileManagerParameter = p.Named<FileManagerNamedParameter>(AppDefaults.NAMED_PARAMETER_FILEMANAGER);
        var dataAccess = ctx.Resolve<IUnitOfWork>();
        var allWorkersEntities = GetRequiredWorkerEntities(dataAccess);
        var assemblyScan = ctx.Resolve<IAssemblyScanService>();

        List<IWorkable> workables = new List<IWorkable>();
        foreach (var workerEntity in allWorkersEntities)
        {
            var workableType = assemblyScan.GetWorkableType(workerEntity.ClassName);
            if(assemblyScan.ImplementsFileWorkable(workableType) || assemblyScan.ImplementsSharedWorkable(workableType))
            {
                IWorkable? workable = null;
                bool isFirstWorker = workerEntity.ID == fileManagerParameter.Manager.FirstWorkerID;

                //Enforce that next worker | pass or failure | implements the correct interfaces
                var workersToCheck = new List<int?>() { workerEntity.PassWorker, workerEntity.FailWorker };
                CheckNextWorkableType(workersToCheck, allWorkersEntities, assemblyScan.GetWorkableType, 
                    assemblyScan.ImplementsFileWorkable, 
                    assemblyScan.ImplementsSharedWorkable);

                if(assemblyScan.ImplementsFileWorkable(workableType))
                {
                    workable = ctx.Resolve<IFileWorkable>(new NamedParameter(
                        AppDefaults.NAMED_PARAMETER_FILEWORKER, new FileWorkerNamedParameter(workerEntity)));

                    WorkerModule.SetRequiredPropertiesAndParameters((IFileWorkable)workable, isFirstWorker, workerEntity,
                        fileManagerParameter.Directory, fileManagerParameter.File);
                }
                else 
                {
                    //SharedWorkable
                    workable = ctx.Resolve<ISharedWorkable>(new NamedParameter(
                        AppDefaults.NAMED_PARAMETER_SHAREDWORKER, new SharedWorkerNamedParameter(workerEntity)));

                    WorkerModule.SetRequiredPropertiesAndParameters((ISharedWorkable)workable, isFirstWorker, workerEntity);
                }

                workables.Add(workable);
            }
        }

        var manageableType = assemblyScan.GetFileManageableType(fileManagerParameter.Manager.ClassName);
        IFileManageable manageableInstance = (IFileManageable)Activator.CreateInstance(manageableType, new object[]
        {
            CreateServiceBox(ctx, manageableType, workables)
        })!;

        SetRequiredPropertiesAndParameters(manageableInstance,
            fileManagerParameter.Manager, fileManagerParameter.EventGeneratorId,
            fileManagerParameter.SkipIfBusyStrategy, fileManagerParameter.Directory, fileManagerParameter.File);

        return manageableInstance;
    }

    private ICompanyManageable CreateCompanyManageable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var companyManagerParameter = p.Named<CompanyManagerNamedParameter>(AppDefaults.NAMED_PARAMETER_COMPANYMANAGER);
        var dataAccess = ctx.Resolve<IUnitOfWork>();
        var allWorkersEntities = GetRequiredWorkerEntities(dataAccess);
        var assemblyScan = ctx.Resolve<IAssemblyScanService>();

        List<IWorkable> workables = new List<IWorkable>();
        foreach (var workerEntity in allWorkersEntities)
        {
            var workableType = assemblyScan.GetWorkableType(workerEntity.ClassName);
            if (assemblyScan.ImplementsCompanyWorkable(workableType) || assemblyScan.ImplementsSharedWorkable(workableType))
            {
                IWorkable? workable = null;
                bool isFirstWorker = workerEntity.ID == companyManagerParameter.Manager.FirstWorkerID;

                //Enforce that next worker | pass or failure | implements the correct interfaces
                var workersToCheck = new List<int?>() { workerEntity.PassWorker, workerEntity.FailWorker };
                CheckNextWorkableType(workersToCheck, allWorkersEntities, assemblyScan.GetWorkableType,
                    assemblyScan.ImplementsCompanyWorkable,
                    assemblyScan.ImplementsSharedWorkable);

                if(assemblyScan.ImplementsCompanyWorkable(workableType))
                {
                    workable = ctx.Resolve<ICompanyWorkable>(new NamedParameter(
                        AppDefaults.NAMED_PARAMETER_COMPANYWORKER, new CompanyWorkerNamedParameter(workerEntity)));

                    WorkerModule.SetRequiredPropertiesAndParameters((ICompanyWorkable)workable, isFirstWorker, workerEntity,
                        companyManagerParameter.Query, companyManagerParameter.Connection);
                }
                else
                {
                    //SharedWorkable
                    workable = ctx.Resolve<ISharedWorkable>(new NamedParameter(
                        AppDefaults.NAMED_PARAMETER_SHAREDWORKER, new SharedWorkerNamedParameter(workerEntity)));

                    WorkerModule.SetRequiredPropertiesAndParameters((ISharedWorkable)workable, isFirstWorker, workerEntity);
                }

                workables.Add(workable);
            }
        }

        var manageableType = assemblyScan.GetCompanyManageableType(companyManagerParameter.Manager.ClassName);
        ICompanyManageable manageableInstance = (ICompanyManageable)Activator.CreateInstance(manageableType, new object[]
        {
            CreateServiceBox(ctx, manageableType, workables)
        })!;

        SetRequiredPropertiesAndParameters(manageableInstance,
            companyManagerParameter.Manager, companyManagerParameter.EventGeneratorId,
            companyManagerParameter.SkipIfBusyStrategy, companyManagerParameter.Query, companyManagerParameter.Connection);

        return manageableInstance;
    }

    [Obsolete("Use a specific manager instead.")]
    private IManageable CreateManageable(Autofac.IComponentContext ctx, IEnumerable<Autofac.Core.Parameter> p)
    {
        var managerParameter = p.Named<ManagerNamedParameter>(AppDefaults.NAMED_PARAMETER_MANAGER);
        var dataAccess = ctx.Resolve<IUnitOfWork>();
        var allWorkersEntities = GetRequiredWorkerEntities(dataAccess);
        var assemblyScan = ctx.Resolve<IAssemblyScanService>();

        List<IWorkable> workables = new List<IWorkable>();
        foreach (var workerEntity in allWorkersEntities)
        {
            var workable = ctx.Resolve<IWorkable>(new NamedParameter(
                AppDefaults.NAMED_PARAMETER_WORKER, new WorkerNamedParameter(workerEntity)));

            bool isFirstWorker = workerEntity.ID == managerParameter.Manager.FirstWorkerID;

            //Enforce that next worker | pass or failure | implements the correct interfaces
            var workersToCheck = new List<int?>() { workerEntity.PassWorker, workerEntity.FailWorker };
            CheckNextWorkableType(workersToCheck, allWorkersEntities, assemblyScan.GetWorkableType,
                assemblyScan.ImplementsWorkable);

            WorkerModule.SetRequiredPropertiesAndParameters(workable, isFirstWorker, workerEntity);

            workables.Add(workable);
        }

        var manageableType = assemblyScan.GetManageableType(managerParameter.Manager.ClassName);
        IManageable manageableInstance = (IManageable)Activator.CreateInstance(manageableType, new object[]
        {
            CreateServiceBox(ctx, manageableType, workables)
        })!;

        SetRequiredPropertiesAndParameters(manageableInstance,
            managerParameter.Manager, managerParameter.EventGeneratorId, managerParameter.SkipIfBusyStrategy);

        return manageableInstance;
    }

    private IManagerServiceBox CreateServiceBox(IComponentContext ctx, Type manageableType, List<IWorkable> workables)
    {
        var serviceBox = new ManagerServiceBox();
        serviceBox.Add((ILogger)ctx.Resolve(typeof(ILogger<>).MakeGenericType(manageableType)));
        serviceBox.Add(ctx.Resolve<IEncryptorService>());
        serviceBox.Add(workables.AsEnumerable());
        serviceBox.Add(ctx.Resolve<IConfigurationService>());
        serviceBox.Add(ctx.Resolve<IUnitOfWork>());
        serviceBox.Add(ctx.Resolve<ISboServiceLayerPoolService>());

        return serviceBox;
    }

    private void CheckNextWorkableType(List<int?> workersToCheck, List<WorkerEntity> allWorkersEntities,
    Func<string, Type> getType, params Func<Type, bool>[] implementsTypes)
    {
        foreach (var workerToCheck in workersToCheck)
        {
            
            var nextWorkerEntity = allWorkersEntities.FirstOrDefault(nw => nw.ID == workerToCheck);
            if (nextWorkerEntity != null)
            {
                Type workableType = getType(nextWorkerEntity.ClassName);
                bool allowed = implementsTypes.Any((impType) => impType(workableType));
                if (allowed == false)
                {
                    throw new IntegrationException($"A next/fail worker [{nextWorkerEntity.ID}:{nextWorkerEntity.Name}] was informed although it cannot be used because of the type it belongs.");
                }
            }
            else
            {
                if (workerToCheck.HasValue)
                {
                    throw new IntegrationException($"Worker ID [{workerToCheck.Value}] doesn't exist.");
                }
            }
        }
    }

    private void SetRequiredPropertiesAndParameters(IFileManageable manageableInstance, ManagerEntity managerEntity,
        int eventGeneratorKey, bool skipIfBusyStrategy, DirectoryInfo directory, FileInfo file)
    {
        SetRequiredPropertiesAndParameters(manageableInstance, managerEntity, eventGeneratorKey, skipIfBusyStrategy);

        var fileManagerInitParameters = new object?[2] {
            directory,
            file
        };

        var methodInfo = ReflectionUtil.GetMethods(manageableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_MANAGER_FILE)
            .FirstOrDefault();

        if (methodInfo != null)
        {
            methodInfo.Invoke(manageableInstance, fileManagerInitParameters);
        }
    }

    private void SetRequiredPropertiesAndParameters(ICompanyManageable manageableInstance, ManagerEntity managerEntity,
    int eventGeneratorKey, bool skipIfBusyStrategy, string? query, ConnectionEntity connection)
    {
        SetRequiredPropertiesAndParameters(manageableInstance, managerEntity, eventGeneratorKey, skipIfBusyStrategy);

        var companyManagerInitParameters = new object?[2] {
            query,
            connection
        };

        var methodInfo = ReflectionUtil.GetMethods(manageableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_MANAGER_COMPANY)
            .FirstOrDefault();

        if (methodInfo != null)
        {
            methodInfo.Invoke(manageableInstance, companyManagerInitParameters);
        }
    }

    private void SetRequiredPropertiesAndParameters(IManageable manageableInstance, ManagerEntity managerEntity, 
        int eventGeneratorKey, bool skipIfBusyStrategy)
    {
        var managerInitParameters = new object?[4] {
            managerEntity.ID,
            managerEntity.Name,
            eventGeneratorKey,
            skipIfBusyStrategy
        };

        var methodInfo = ReflectionUtil.GetMethods(manageableInstance.GetType())
            .Where(m => m.Name == AppDefaults.PRIVATE_METHOD_MANAGER)
            .FirstOrDefault();

        if (methodInfo != null)
        {
            methodInfo.Invoke(manageableInstance, managerInitParameters);
        }

        //set all parameters if any
        SetParameters(managerEntity, manageableInstance);
    }

    private List<WorkerEntity> GetRequiredWorkerEntities(IUnitOfWork dataAccess)
    {
        var allWorkersEntities = dataAccess.WorkerRepository.GetAll().Where(w => w.Active).ToList();
        if (allWorkersEntities == null || allWorkersEntities.Count == 0)
        {
            throw new IntegrationException("No workers were found inside Autofac Register for a manager");
        }

        return allWorkersEntities;
    }

    private void SetParameters(ManagerEntity managerEntity, IManageable manageableInstance)
    {
        if (managerEntity.Parameters != null && managerEntity.Parameters.Count > 0)
        {
            var wrongParameter = managerEntity.Parameters.FirstOrDefault(p => string.IsNullOrEmpty(p.Name));
            if (wrongParameter != null)
            {
                throw new IntegrationException($"Parameter name {wrongParameter.Name} when setting parameters for Manager is null");
            }

            Type manageableType = manageableInstance.GetType();
            foreach (var parameter in managerEntity.Parameters)
            {
                try
                {
                    manageableType.GetProperty(parameter.Name)?.SetValue(manageableInstance,
                        ParameterCaster.CastParameterValue(parameter.Type, parameter.Value));
                }
                catch (Exception ex)
                {
                    throw new ParameterCastException($"Manager [{manageableInstance.Key}:{manageableInstance.Name}]. Error while parsing parameter name {parameter.Name} for Manager. {ex.Message}",
                        parameter.Name!, ex);
                }
            }
        }
    }
}