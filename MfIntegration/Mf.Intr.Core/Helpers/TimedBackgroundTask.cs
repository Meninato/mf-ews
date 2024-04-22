using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Helpers;


public class TimedBackgroundTask : IDisposable
{
    private Task _timerTask = null!;
    private PeriodicTimer? _pTimer;
    private TimeSpan _interval;
    private CancellationTokenSource? _cts;
    private bool _runOnce;
    private TimeSpan? _stopAfter;
    private TimeSpan _delayedInterval;

    public int DelayedTimeBeforeFileToBeReady => _delayedInterval.Seconds;

    public TimedBackgroundTaskStatus Status { get; private set; }

    public TimedBackgroundTask(Task timerTask, TimeSpan interval) 
    {
        Initialize(timerTask, interval, false, null);
    }

    public TimedBackgroundTask(Task timerTask, TimeSpan interval, bool runOnce)
    {
        Initialize(timerTask, interval, runOnce, null);
    }

    public TimedBackgroundTask(Task timerTask, TimeSpan interval, TimeSpan stopAfter)
    {
        Initialize(timerTask, interval, false, stopAfter);
    }

    public async Task StartDelayedAsync(TimeSpan delay)
    {
        if(_stopAfter.HasValue) 
        {
            throw new IntegrationException("You cannot add a delay when it was initialized with stopAfter parameter.");
        }

        _delayedInterval = _delayedInterval.Add(delay);

        await StartAsync(_delayedInterval, new CancellationTokenSource());
    }

    public async Task StartAsync()
    {
        var cts = _stopAfter.HasValue ? 
            new CancellationTokenSource(_stopAfter.Value) : 
            new CancellationTokenSource();

        if (_runOnce && _stopAfter.HasValue)
        {
            throw new IntegrationException("You cannot use the combination run once true with stop after.");
        }

        await StartAsync(_interval, cts);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _pTimer?.Dispose();
        Status = TimedBackgroundTaskStatus.Idle;
    }

    public void ResetDelayedTime()
    {
        _delayedInterval = TimeSpan.FromSeconds(0);
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
            _pTimer?.Dispose();
            _cts?.Dispose();
        }
        // free native resources if there are any.
    }

    private void Initialize(Task timerTask, TimeSpan interval, bool runOnce, TimeSpan? stopAfter)
    {
        if (stopAfter.HasValue && _interval >= stopAfter.Value)
        {
            throw new IntegrationException("interval parameter cannot be greater than stopAfter parameter.");
        }

        _timerTask = timerTask;
        _interval = interval;
        _runOnce = runOnce;
        _stopAfter = stopAfter;
        _delayedInterval = _delayedInterval.Add(_interval);
        Status = TimedBackgroundTaskStatus.Idle;
    }

    private async Task StartAsync(TimeSpan interval, CancellationTokenSource cts)
    {
        Stop();

        _pTimer = new PeriodicTimer(interval);
        _cts = cts;

        try
        {
            await DoWorkAsync();
        }
        catch (OperationCanceledException) { }
    }

    private async Task DoWorkAsync()
    {
        while (await _pTimer!.WaitForNextTickAsync(_cts!.Token))
        {
            Status = TimedBackgroundTaskStatus.Running;

            await Task.Run(() => {
                _timerTask.Start();
                _timerTask.Wait();
                Status = TimedBackgroundTaskStatus.Idle;
            });

            if (_runOnce)
            {
                Stop();
            }
        }
    }
}
