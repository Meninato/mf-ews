using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sap.EntityFrameworkCore.Hana;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Plugins.Hana;

public class HanaDbContext : DbContext, IHanaDbContext
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly AppOptions _appOptions;
    public HanaDbContext(ILoggerFactory loggerFactory, IOptions<AppOptions> settingWrapper)
    {
        //Check if ILoggerFactory was injected
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        //Check if AppOptions was injected
        _appOptions = settingWrapper == null
            ? throw new ArgumentNullException(nameof(settingWrapper))
            : settingWrapper.Value;

        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbConnection GetDbConnection()
    {
        return this.Database.GetDbConnection();
    }

    public void SetConnectionString(string connectionString)
    {
        this.SetConnectionString(connectionString);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (_appOptions.EnableEFCoreLogging)
        {
            options.UseLoggerFactory(_loggerFactory);
        }

        options.UseHana();
    }
}
