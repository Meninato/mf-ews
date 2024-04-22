using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Mf.Intr.Application.Injection.Serilog;
using Mf.Intr.Core.DataAccess;
using Microsoft.Extensions.Logging;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Application.Injection.Configuration;
using Mf.Intr.Application.Injection.Worker;
using Mf.Intr.Application.Injection.Manager;
using Mf.Intr.Application.Injection.AssemblyScan;
using Mf.Intr.Application.Injection.GeneralComponents;
using Mf.Intr.Application.Helpers;
using Mf.Intr.Application.Services;
using Microsoft.Extensions.Options;
using Mf.Intr.Core.Options;
using Serilog;
using Mf.Intr.Application.Injection.SboServiceLayer;

namespace Mf.Intr.Application;

public sealed class Startup
{
    private readonly string _currentDirPath;
    private readonly string _logDirPath;
    private readonly string _assemblyDirPath;
    private readonly string _pluginDirPath;
    private readonly string _pluginHanaDirPath;
    private readonly string _configDirPath;

    public static IContainer Container = null!;

    public Startup()
    {
        Quartz.Logging.LogProvider.IsDisabled = true;

        _currentDirPath = GetCurrentDirectory();
        _logDirPath = Path.Combine(_currentDirPath, AppDefaults.LOGGER_FOLDER);
        _assemblyDirPath = Path.Combine(_currentDirPath, AppDefaults.ASSEMBLY_FOLDER);
        _configDirPath = Path.Combine(_currentDirPath, AppDefaults.CONFIG_FOLDER);
        _pluginDirPath = Path.Combine(_currentDirPath, AppDefaults.PLUGIN_FOLDER);
        _pluginHanaDirPath = Path.Combine(_pluginDirPath, AppDefaults.PLUGIN_HANA_FOLDER);

        EnsureFoldersAreCreated();
        EnsureSettingsAreCreated();
        RegisterComponents();
    }

    private string GetCurrentDirectory()
    {
        string? dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if(string.IsNullOrEmpty(dir))
        {
            dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName);
        }

        if(dir == null)
        {
            throw new IntegrationException("Not possible to get the path of the executing assembly");
        }

        return dir;
    }

    private void RegisterComponents()
    {
        ContainerBuilder builder = new ContainerBuilder();

        builder.RegisterConfiguration(Path.Combine(_currentDirPath, AppDefaults.CONFIG_FOLDER));
        builder.RegisterSerilog(Path.Combine(_logDirPath, AppDefaults.LOGGER_NAME));
        builder.RegisterWorker();
        builder.RegisterManager();
        builder.RegisterAssemblyScan(_assemblyDirPath, _pluginDirPath);
        builder.RegisterGeneralComponents(_currentDirPath);
        builder.RegisterSboServiceLayer();

        Container = builder.Build();

        EnsureDatabaseIsCreated();
    }

    private void EnsureSettingsAreCreated()
    {
        if(Directory.Exists(_configDirPath))
        {
            var filenames = Directory.EnumerateFiles(_configDirPath)
                .Select(f => Path.GetFileName(f))
                .ToList();

            if(filenames.Contains(AppDefaults.SETTINGS_FILENAME) == false)
            {
                string file = Path.Combine(_configDirPath, AppDefaults.SETTINGS_FILENAME);
                string content =   "{\r\n  \"Intr.Application\": {\r\n    \"EnableEFCoreLogging\": false, \r\n    \"LogOutputType\": \"Json\" \r\n  }\r\n}";
                CreateFile(file, content);
            }

            if(filenames.Contains(AppDefaults.SETTINGS_DEV_FILENAME) == false)
            {
                string file = Path.Combine(_configDirPath, AppDefaults.SETTINGS_DEV_FILENAME);
                string content = "{\n  \n}";
                CreateFile(file, content);
            }

            if(filenames.Contains(AppDefaults.SETTINGS_PROD_FILENAME) == false)
            {
                string file = Path.Combine(_configDirPath, AppDefaults.SETTINGS_PROD_FILENAME);
                string content = "{\n  \n}";
                CreateFile(file, content);
            }
        }
    }

    private void EnsureFoldersAreCreated()
    {
        List<string> dirs = new List<string>() { 
            _logDirPath, 
            _assemblyDirPath, 
            _configDirPath, 
            _pluginDirPath,
            _pluginHanaDirPath
        };

        dirs.ForEach((path) =>
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        });
    }

    private void EnsureDatabaseIsCreated()
    {
        using (var scope = Container.BeginLifetimeScope())
        {
            var dbContext = scope.Resolve<IntrDbContext>();
            var logger = scope.Resolve<ILogger<Startup>>();
            if (dbContext.Database.EnsureCreated())
            {
                string text = "SQLite config db was created. Stopping integration... please now setup manually the db settings.";
                logger.LogInformation(text);

                throw new DatabaseCreationException(text);
            }
        }
    }
    private void CreateFile(string file, string content)
    {
        using (var fs = File.Create(file))
        {
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(content);
            }
        }
    }
}
