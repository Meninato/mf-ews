using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Helpers;

public class PluginLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;
    private readonly FileInfo _assemblyFileInfo;
    private readonly string _dependenciesDirPath;

    /// <summary>
    /// Load an assembly and it's dependencies and they must be located at the same place.
    /// Or if you want more control you can add Resolving event to solve the case when it is not found.
    /// </summary>
    /// <param name="assemblyPath">Assembly to load</param>
    public PluginLoadContext(string assemblyPath)
    {
        _assemblyFileInfo = new FileInfo(assemblyPath);

        if (_assemblyFileInfo.Exists == false)
        {
            throw new IntegrationException($"Assembly file not found {_assemblyFileInfo.FullName}");
        }

        _dependenciesDirPath = string.Empty;
        _resolver = new AssemblyDependencyResolver(assemblyPath);
    }

    /// <summary>
    ///  Load an assembly and it's dependencies.
    ///  The dependencies can be at the same level as the assembly or you can specify a specific folder.
    /// </summary>
    /// <param name="assemblyPath">Assembly to load</param>
    /// <param name="dependenciesDirPath">Folder of the assembly dependencies.</param>
    /// <exception cref="IntegrationException">Throws an exception if assemblyPath and dependenciesDirPath don't exist</exception>
    public PluginLoadContext(string assemblyPath, string dependenciesDirPath)
    {
        _assemblyFileInfo = new FileInfo(assemblyPath);
        _dependenciesDirPath = dependenciesDirPath;

        if(_assemblyFileInfo.Exists == false)
        {
            throw new IntegrationException($"Assembly file not found {_assemblyFileInfo.FullName}");
        }

        if(Directory.Exists(_dependenciesDirPath) == false)
        {
            throw new IntegrationException($"Dependencies directory not found {_dependenciesDirPath}");
        }

        _resolver = new AssemblyDependencyResolver(assemblyPath);

        Resolving += PluginLoadContext_Resolving;
    }

    private Assembly? PluginLoadContext_Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        Assembly? assembly = null;
        string assemblyDependency = Path.Combine(_dependenciesDirPath, $"{assemblyName.Name}.dll");
        if(File.Exists(assemblyDependency))
        {
            assembly = context.LoadFromAssemblyPath(assemblyDependency);
        }
        return assembly;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }
        return IntPtr.Zero;
    }
}