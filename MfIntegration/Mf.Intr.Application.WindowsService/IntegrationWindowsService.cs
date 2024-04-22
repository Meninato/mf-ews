using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.EventLog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace Mf.Intr.Application.WindowsService;

[SupportedOSPlatform("windows")]
public class IntegrationWindowsService : IHostedService
{
    private IntegrationApp? _integrationApp;
    private ILogger? _appLogger;
    private ILogger? _windowsLogger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        bool startSchedulers = true;
        _windowsLogger = new EventLogLoggerProvider().CreateLogger("integration");

        _windowsLogger.LogInformation("Starting Integration service.");

        try
        {
            _integrationApp = new IntegrationApp();
            _appLogger = _integrationApp.Logger;

            _appLogger.LogInformation("Integration has started.");
        }
        catch(Exception ex)
        {
            startSchedulers = false;
            _windowsLogger.LogCritical(ex, "Failed to start the Integration service.");
        }

        if(startSchedulers)
        {
            await StartCompanyScheduler();
            await StartFileScheduler();
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _windowsLogger!.LogInformation("Stopping Integration.");

        if(_integrationApp != null)
        {
            _appLogger!.LogInformation("Integration has stopped.");

            await _integrationApp.CompanyScheduler.Stop();
            await _integrationApp.FileScheduler.Stop();
        }

        await Task.CompletedTask;
    }

    private async Task StartCompanyScheduler()
    {
        try
        {
            await _integrationApp!.CompanyScheduler.Start();
        }
        catch (Exception ex)
        {
            _appLogger!.LogCritical(ex, "Something bad happened... stopping CompanyScheduler");

            await _integrationApp!.CompanyScheduler.Stop();
        }
    }

    private async Task StartFileScheduler()
    {
        try
        {
            await _integrationApp!.FileScheduler.Start();
        }
        catch (Exception ex)
        {
            _appLogger!.LogCritical(ex, "Something bad happened... stopping FileScheduler");

            await _integrationApp!.FileScheduler.Stop();
        }
    }
}
