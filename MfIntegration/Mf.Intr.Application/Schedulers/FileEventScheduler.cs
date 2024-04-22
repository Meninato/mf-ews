using Mf.Intr.Application.Jobs;
using Mf.Intr.Application.Services;
using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Schedulers;

public class FileEventScheduler : IIntrScheduler
{
    private readonly IUnitOfWork _dataAccess;
    private readonly Dictionary<string, FileWatcherJob> _jobs;
    private readonly ILogger _logger;

    public FileEventScheduler(IUnitOfWork dataAccess, ILogger<FileEventScheduler> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
        _jobs = new Dictionary<string, FileWatcherJob>();
    }

    public async Task Prepare()
    {
        _logger.LogInformation("Creating scheduler and setting up jobs and triggers.");

        var fileEventGenerators = await GetActiveFileEvents();

        foreach (var fileEvtGen in fileEventGenerators)
        {
            var fileEvent = fileEvtGen.FileEvent!;
            var fileWatcher = new FileWatcherJob(fileEvent);

            AddJobIntoCache(fileEvent.JobKey, fileWatcher);
        }
    }

    public async Task Start()
    {
        await Prepare();

        _logger.LogInformation("Running all jobs to listen all directories.");

        if(_jobs.Count > 0)
        {
            foreach (var job in _jobs)
            {
                job.Value.Watch();
            }
        }
        else
        {
            _logger.LogInformation("There are no jobs to execute.");
        }

        await Task.CompletedTask;
    }

    public async Task StartOnce(string jobKey)
    {
        CheckScheduler();

        _logger.LogInformation("Starting job [{job}] to listen one directory.", jobKey);

        if(_jobs.TryGetValue(jobKey, out var job) == false)
        {
            throw new IntegrationException($"Job idenfied by key [{jobKey}] was not found.");
        }

        job.Watch();

        await Task.CompletedTask;
    }

    public async Task StartOnce()
    {
        CheckScheduler();

        _logger.LogInformation("Firing all jobs to listen all directories.");

        foreach (var job in _jobs)
        {
            job.Value.Watch();
        }

        await Task.CompletedTask;
    }

    public async Task Stop()
    {
        _logger.LogInformation("Stopping scheduler.");

        foreach (var job in _jobs)
        {
            job.Value.Dispose();
        }

        _jobs.Clear();

        await Task.CompletedTask;
    }

    private void AddJobIntoCache(string jobKey, FileWatcherJob fileWatcher)
    {
        _jobs.Add(jobKey, fileWatcher);
    }

    private void CheckScheduler()
    {
        if (_jobs == null || _jobs.Count == 0)
        {
            throw new IntegrationException("Please, call Prepare method first.");
        }
    }

    private async Task<List<EventGeneratorEntity>> GetActiveFileEvents()
    {
        var eventsGenerators = await _dataAccess.EventGeneratorRepository.GetAllAsync();
        eventsGenerators = eventsGenerators
            .Where(
                ev => ev.Active &&
                ev.Type == EventGeneratorType.File);

        var eg = eventsGenerators.FirstOrDefault(ev => ev.FileEvent == null);
        if(eg != null)
        {
            throw new IntegrationException($"Event generator [{eg.ID}:{eg.Name}] must have a File Event configured");
        }

        return eventsGenerators.ToList();
    }
}
