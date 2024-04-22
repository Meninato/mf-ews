using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mf.Intr.Core.Helpers;
using Mf.Intr.Core.Interfaces.Services;
using System.Runtime.CompilerServices;
using Autofac;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Schedulers;
using Microsoft.Extensions.Logging;
using Mf.Intr.Application.Helpers.NamedParameters;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Helpers.Extensions;

namespace Mf.Intr.Application.Jobs;

public class FileWatcherJob
{
    private FileSystemWatcher _watcher = null!;
    private List<string> _fileTypes = null!;
    private DirectoryInfo _directoryInfo = null!;
    private ConcurrentDictionary<string, TimedBackgroundTask> _filesEvents = null!;
    private FileEventEntity _fileEvent;

    public FileWatcherJob(FileEventEntity fileEvent)
    {
        _fileEvent = fileEvent;

        _directoryInfo = new DirectoryInfo(_fileEvent.Directory);
        if (_directoryInfo.Exists == false)
        {
            throw new IntegrationException("Directory path doesn't exist.");
        }

        if (string.IsNullOrEmpty(_fileEvent.FileTypes))
        {
            throw new IntegrationException("You have to specify what file types to filter.");
        }

        _fileTypes = fileEvent.FileTypes.Split('|').ToList();

        if (_fileTypes.Count == 0)
        {
            throw new IntegrationException("You have to specify what file types to filter.");
        }

        if (_fileEvent.TimeForFileToBeReady < 5)
        {
            throw new IntegrationException("You have to specify a time for file to be ready greater than or equal 5 seconds.");
        }
    }

    public void Watch()
    {
        CallWatch();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            _watcher?.Dispose();
        }
        // free native resources if there are any.
    }

    private void CallWatch()
    {
        _watcher = new FileSystemWatcher();
        _filesEvents = new ConcurrentDictionary<string, TimedBackgroundTask>();

        _watcher.Path = _directoryInfo.FullName;

        _watcher.IncludeSubdirectories = false;
        _watcher.InternalBufferSize = 64 * 1024;
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        _fileTypes.ForEach(f => _watcher.Filters.Add($"*{f}"));

        _watcher.Created += OnFileChanged;
        _watcher.Changed += OnFileChanged;

        _watcher.EnableRaisingEvents = true;
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ILogger? logger = Startup.Container.Resolve<ILogger<FileWatcherJob>>();

        try
        {
            await CallOnFileChanged(logger, e);
        }
        catch (Autofac.Core.DependencyResolutionException sourceEx)
        {
            var integrationEx = sourceEx.GetInnerExceptions().Where(e => e is IntegrationException).FirstOrDefault();
            var ex = integrationEx ?? sourceEx;
            logger.LogError(integrationEx ?? ex, "Failed at dependency resolution. {errorMsg}", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong... {errorMsg}", ex.Message);
        }
        finally
        {
            logger = null;
        }
    }

    private async Task CallOnFileChanged(ILogger logger, FileSystemEventArgs e)
    {
        logger.LogInformation("Job [{jobKey}] identified by [{jobName}] is running now", _fileEvent.JobKey, _fileEvent.EventGenerator.Name);

        using (var scope = Startup.Container.BeginLifetimeScope())
        {
            DateTime eventTime = DateTime.Now;
            FileInfo fileInfo = new FileInfo(e.FullPath);

            if (string.IsNullOrEmpty(e.Name) == false && fileInfo.Exists)
            {
                logger.LogInformation("Handling file \"{file}\" ", e.Name);

                if (_filesEvents.TryGetValue(e.Name, out TimedBackgroundTask? bgTask))
                {
                    //The task may take longer skip further events for that file until it finish the previous
                    if (bgTask.Status == TimedBackgroundTaskStatus.Idle)
                    {
                        logger.LogInformation("File is not ready delaying to {extraTime} seconds.",
                            bgTask.DelayedTimeBeforeFileToBeReady + _fileEvent.TimeForFileToBeReady);

                        await bgTask.StartDelayedAsync(TimeSpan.FromSeconds(_fileEvent.TimeForFileToBeReady));
                    }
                    else
                    {
                        logger.LogWarning("The file is already being handled in a previous event.");
                    }
                }
                else
                {
                    logger.LogInformation("File will be ready in {seconds} seconds.", _fileEvent.TimeForFileToBeReady);

                    var manageablePool = scope.Resolve<IManageablePoolService>();

                    IManageable manager = scope.Resolve<IFileManageable>(
                        new NamedParameter(AppDefaults.NAMED_PARAMETER_FILEMANAGER, GetFileManagerNamedParameter(_fileEvent, 
                            _directoryInfo, fileInfo)));

                    manager = manageablePool.GetFromCache(manager, SchedulerType.File, _fileEvent.ID);

                    Task task = new Task(() =>
                    {
                        logger.LogInformation("Manager [{key}:{name}] is ready and it's gonna work.", manager.Key, manager.Name);

                        try { manager.StartWork(); }
                        catch (Exception) { throw; }
                        finally { RemoveFileFromCache(e.Name); }
                    });

                    TimedBackgroundTask newBgTask =
                        new TimedBackgroundTask(task, TimeSpan.FromSeconds(_fileEvent.TimeForFileToBeReady), runOnce: true);
                    _filesEvents.TryAdd(e.Name, newBgTask);

                    await newBgTask.StartAsync();
                }
            }
        }
    }

    private FileManagerNamedParameter GetFileManagerNamedParameter(FileEventEntity fileEvent, 
        DirectoryInfo directory, FileInfo file)
    {
        return new FileManagerNamedParameter(
            fileEvent.EventGenerator.Manager,
            fileEvent.EventGeneratorID,
            fileEvent.SkipIfBusyStrategy,
            directory,
            file);
    }

    private void RemoveFileFromCache(string filename)
    {
        _filesEvents.TryRemove(filename, out TimedBackgroundTask? bgTaskRemove);
    }
}
