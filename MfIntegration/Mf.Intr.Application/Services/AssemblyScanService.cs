using Mf.Intr.Application.Helpers;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Helpers;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services;

public sealed class AssemblyScanService : IAssemblyScanService
{
    private List<Type> _workableInterfaces;
    private List<Type> _manageableInterfaces;
    private List<Type> _companyWorkableInterfaces;
    private List<Type> _companyManageableInterfaces;
    private List<Type> _fileWorkableInterfaces;
    private List<Type> _fileManageableInterfaces;
    private List<Type> _sharedWorkableInterfaces;
    private List<Type> _hanaPluginInterfaces;
    private List<Assembly> _assemblies;

    public AssemblyScanService()
    {
        _workableInterfaces = new List<Type>() { typeof(IWorkable<>), typeof(IWorkable<,>) };
        _manageableInterfaces = new List<Type>() { typeof(IManageable) };
        _companyManageableInterfaces = new List<Type>() { typeof(ICompanyManageable) };
        _companyWorkableInterfaces = new List<Type>() { typeof(ICompanyWorkable) };
        _fileWorkableInterfaces = new List<Type>() { typeof(IFileWorkable) };
        _fileManageableInterfaces = new List<Type>() { typeof(IFileManageable) };
        _sharedWorkableInterfaces = new List<Type>() { typeof(ISharedWorkable) };
        _hanaPluginInterfaces = new List<Type>() { typeof(IHanaDbContext) };

        _assemblies = new List<Assembly>();
    }

    public Type GetWorkableType(string? classType)
    {
        if (string.IsNullOrEmpty(classType))
        {
            throw new IntegrationException($"Class name is missing for Workers.");
        }
        
        Type? workableType = GetType(classType);

        if (workableType == null)
        {
            throw new IntegrationException($"Type {classType} was not found in the assembly or it is not a concrete class. Also, check if the DLL was copied.");
        }

        if (ImplementsWorkable(workableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from Worker<> in Type {classType}");
        }

        return workableType;
    }

    public Type GetFileWorkableType(string? classType)
    {
        Type workableType = GetWorkableType(classType);

        if (ImplementsFileWorkable(workableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from FileWorker<> in Type {classType}");
        }

        return workableType;
    }

    public bool ImplementsHanaPluginContext(Type hanaPlugin)
    {
        return hanaPlugin.GetInterfaces()
            .Any(i => i.IsGenericType == false && _hanaPluginInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public Type GetHanaPluginType(string? classType)
    {
        if (string.IsNullOrEmpty(classType))
        {
            throw new IntegrationException($"Class name is missing for hana plugin.");
        }

        Type? hanaPluginType = GetType(classType);

        if (hanaPluginType == null)
        {
            throw new IntegrationException($"Type {classType} was not found in the assembly or it is not a concrete class. Also, check if the DLL was copied to hana plugin folder.");
        }

        if (ImplementsHanaPluginContext(hanaPluginType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from IHanaDbContext in Type {classType}");
        }

        return hanaPluginType;
    }

    public bool ImplementsFileWorkable(Type workableType)
    {
        return workableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _fileWorkableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public Type GetCompanyWorkableType(string? classType)
    {
        Type workableType = GetWorkableType(classType);

        if(ImplementsCompanyWorkable(workableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from CompanyWorker<> in Type {classType}");
        }

        return workableType;
    }

    public bool ImplementsCompanyWorkable(Type workableType)
    {
        return workableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _companyWorkableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public bool ImplementsWorkable(Type workableType)
    {
        return workableType.GetInterfaces()
            .Any(i => i.IsGenericType &&
                _workableInterfaces.Any(gi => i.GetGenericTypeDefinition().FullName == gi.FullName));
    }

    public bool ImplementsSharedWorkable(Type workableType)
    {
        return workableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _sharedWorkableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public Type GetSharedWorkableType(string? classType)
    {
        Type workableType = GetWorkableType(classType);

        if (ImplementsSharedWorkable(workableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from SharedWorker<> in Type {classType}");
        }

        return workableType;
    }

    public bool ImplementsFileManageable(Type manageableType)
    {
        return manageableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _fileManageableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public Type GetFileManageableType(string? classType)
    {
        Type manageableType = GetManageableType(classType);

        if (ImplementsFileManageable(manageableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from FileManager<> in Type {classType}");
        }

        return manageableType;
    }

    public bool ImplementsCompanyManageable(Type manageableType)
    {
        return manageableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _companyManageableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public Type GetCompanyManageableType(string? classType)
    {
        Type manageableType = GetManageableType(classType);

        if (ImplementsCompanyManageable(manageableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from CompanyManager<> in Type {classType}");
        }

        return manageableType;
    }

    public Type GetManageableType(string? classType)
    {
        if (string.IsNullOrEmpty(classType))
        {
            throw new IntegrationException($"Class name is missing for managers.");
        }

        Type? manageableType = GetType(classType);

        if (manageableType == null)
        {
            throw new IntegrationException($"Type {classType} was not found in the assembly or it is not a concrete class. Also, check if the DLL was copied.");
        }

        if (ImplementsManageable(manageableType) == false)
        {
            throw new IntegrationException($"Please, ensure your class inherits from Manager in Type {classType}");
        }

        return manageableType;
    }

    public bool ImplementsManageable(Type manageableType)
    {
        return manageableType.GetInterfaces()
            .Any(i => i.IsGenericType == false && _manageableInterfaces.Any(mi => i.FullName == mi.FullName));
    }

    public void Load(string directoryPath)
    {
        if (Directory.Exists(directoryPath) == false)
        {
            throw new IntegrationException($"Directory {directoryPath} was not found.");
        }

        foreach (string dll in Directory.GetFiles(directoryPath, "*.dll"))
        {
            string assemblyDependenciesDir = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(dll)}");
            var pluginContext = new PluginLoadContext(dll, assemblyDependenciesDir);
            var assName = new AssemblyName(Path.GetFileNameWithoutExtension(dll));
            var ass = pluginContext.LoadFromAssemblyName(assName);
            _assemblies.Add(ass);
        }
    }

    public void Add(string fileName)
    {
        FileInfo fileInfo = new FileInfo(fileName);

        if (fileInfo.Exists == false)
        {
            throw new IntegrationException($"File {fileName} not found.");
        }

        if (fileName.EndsWith(".dll") == false)
        {
            throw new IntegrationException($"File type {fileName} does not fall within the expected extension .dll");
        }

        //string assemblyDependenciesDir = Path.Combine(fileInfo.DirectoryName!, $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}");
        var pluginContext = new PluginLoadContext(fileInfo.FullName);
        var assName = new AssemblyName(Path.GetFileNameWithoutExtension(fileInfo.Name));
        var ass = pluginContext.LoadFromAssemblyName(assName);
        _assemblies.Add(ass);
    }

    public Type? GetType(string classType)
    {
        Type? type = null;
        foreach (Assembly assembly in _assemblies)
        {
            type = assembly.GetType(classType, false, true);
            if (type != null)
                break;
        }
        return type;
    }
}
