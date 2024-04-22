using Mf.Intr.Application.Jobs;
using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Schedulers;

public class CompanyEventScheduler : IIntrScheduler
{
    private IScheduler? _scheduler;
    private readonly IUnitOfWork _dataAccess;
    private readonly List<Tuple<IJobDetail, ITrigger, string>> _jobs;
    private readonly ILogger _logger;

    public CompanyEventScheduler(IUnitOfWork dataAccess, ILogger<CompanyEventScheduler> logger)
    {
        _dataAccess = dataAccess;
        _jobs = new List<Tuple<IJobDetail, ITrigger, string>>();
        _logger = logger;
    }

    public async Task Prepare()
    {
        await Stop();

        _logger.LogInformation("Creating scheduler and setting up jobs and triggers.");

        _scheduler = await new StdSchedulerFactory().GetScheduler();

        await _scheduler.Start();

        var companyEventGenerators = await GetActiveCompanyEvents();

        foreach (var companyEvtGen in companyEventGenerators)
        {
            foreach (var companyEvent in companyEvtGen.CompanyEvents!)
            {
                string jobid = companyEvent.JobKey;

                IJobDetail job = JobBuilder.Create<CompanyEventJob>()
                    .WithIdentity(jobid, EventGeneratorType.Company.ToString())
                    .Build();

                //var companyEventData = GetCompanyEventJobData(companyEvent);
                job.JobDataMap.Put(AppDefaults.JOB_DATA_COMPANYEVENT, companyEvent);

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"{jobid}_trigger", EventGeneratorType.Company.ToString())
                    .StartNow()
                    .Build();

                AddJobIntoCache(job, trigger, companyEvent.CronExpression);
            }
        }
    }

    public async Task Start()
    {
        await Prepare();

        _logger.LogInformation("Running all jobs.");

        if (_jobs.Count > 0)
        {
            foreach (var cache in _jobs)
            {
                var job = cache.Item1;
                var trigger = cache.Item2;
                var cron = cache.Item3;

                var triggerBuilder = trigger.GetTriggerBuilder();
                var newTrigger = triggerBuilder.WithCronSchedule(cron).Build();

                await _scheduler!.ScheduleJob(job, newTrigger);
            }
        }
        else
        {
            _logger.LogInformation("There are no jobs to execute.");
        }
    }

    public async Task StartOnce(string jobKey)
    {
        CheckScheduler();

        _logger.LogInformation("Firing job [{job}] only once.", jobKey);

        var cachedFound = _jobs.FirstOrDefault(cache => cache.Item1.Key.Name.ToLower() == jobKey.ToLower());
        if(cachedFound == null)
        {
            throw new IntegrationException($"Job idenfied by key [{jobKey}] was not found.");
        }

        await ScheduleOrTrigger(cachedFound.Item1, cachedFound.Item2);
    }

    public async Task StartOnce()
    {
        CheckScheduler();

        _logger.LogInformation("Firing all jobs only once.");

        foreach (var cache in _jobs)
        {
            var job = cache.Item1;
            var trigger = cache.Item2;

            await ScheduleOrTrigger(job, trigger);
        }
    }

    public async Task Stop()
    {
        if (_scheduler != null )
        {
            _logger.LogInformation("Stopping scheduler.");
            _jobs.Clear();
            await _scheduler.Shutdown();
        }
    }

    private async Task ScheduleOrTrigger(IJobDetail job, ITrigger trigger)
    {
        var schedulerJobFound = await _scheduler!.GetJobDetail(job.Key);
        if (schedulerJobFound != null)
        {
            await _scheduler!.TriggerJob(schedulerJobFound.Key);
        }
        else
        {
            await _scheduler!.ScheduleJob(job, trigger);
        }
    }

    //private CompanyEventEntity GetCompanyEventJobData(CompanyEventEntity companyEvent)
    //{
    //    if (companyEvent.EventGenerator.Manager.ID == null)
    //        throw new ArgumentNullException("Manager ID is null for an EventGenerator.");

    //    return new CompanyEventJobData(
    //        companyEvent.EventGenerator.Manager.ID.Value,
    //        companyEvent.EventGenerator.Manager.ClassName,
    //        companyEvent.Query);
    //}

    private void AddJobIntoCache(IJobDetail jobDetail, ITrigger trigger, string cron)
    {
        _jobs.Add(new Tuple<IJobDetail, ITrigger, string>(jobDetail, trigger, cron));
    }

    private void CheckScheduler()
    {
        if (_scheduler == null)
        {
            throw new IntegrationException("Please, call Prepare method first.");
        }
    }

    private async Task<List<EventGeneratorEntity>> GetActiveCompanyEvents()
    {
        var eventsGenerators = await _dataAccess.EventGeneratorRepository.GetAllAsync();
        eventsGenerators = eventsGenerators
            .Where(
                ev => ev.Active &&
                ev.Type == EventGeneratorType.Company);

        var eg = eventsGenerators.FirstOrDefault(ev => ev.CompanyEvents == null || ev.CompanyEvents.Count == 0);
        if(eg != null)
        {
            throw new IntegrationException($"Event generator [{eg.ID}:{eg.Name}] must have a Company Event configured");
        }

        foreach (var evGen in eventsGenerators)
        {
            evGen.CompanyEvents = evGen.CompanyEvents!.Where(company => company.Active).ToList();
        }

        return eventsGenerators.ToList();
    }
}
