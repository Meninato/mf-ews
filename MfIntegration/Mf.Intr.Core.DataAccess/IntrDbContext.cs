using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mf.Intr.Core.Options;
using Mf.Intr.Core.Db.Entities;

namespace Mf.Intr.Core.DataAccess;
public class IntrDbContext : DbContext
{
    private readonly string _dbFolder = "db";
    private readonly string _dbName = "integration.sdb";
    private readonly string _dbPath;
    private readonly ILoggerFactory _loggerFactory;
    private readonly AppOptions _appOptions;

    public DbSet<ConnectionEntity> Connections { get; set; }
    public DbSet<EventGeneratorEntity> EventGenerators { get; set; }
    public DbSet<CompanyEventEntity> CompanyEvents { get; set; }
    public DbSet<WorkerEntity> Workers { get; set; }
    public DbSet<WorkerParameterEntity> WorkerParameters { get; set; }
    public DbSet<ManagerEntity> Managers { get; set; }
    public DbSet<ManagerParameterEntity> ManagerParameters { get; set; }
    public DbSet<SboServiceLayerConnectionEntity> SboServiceLayerConnections { get; set; }

    public IntrDbContext(ILoggerFactory loggerFactory, IOptions<AppOptions> settingWrapper, string currentDirPath)
    {
        //Check if ILoggerFactory was injected
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        //Check if AppOptions was injected
        _appOptions = settingWrapper == null 
            ? throw new ArgumentNullException(nameof(settingWrapper)) 
            : settingWrapper.Value;

        _dbPath = Path.Combine(currentDirPath, _dbFolder, _dbName);
        CreateDbDirectoryIfNotExists(_dbPath);
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if(_appOptions.EnableEFCoreLogging)
        {
            options.UseLoggerFactory(_loggerFactory);
        }
        options.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SboServiceLayerConnectionEntity>(entity =>
        {
            entity.ToTable("INTSBOSLCONNECTION");

            entity.HasKey(e => e.ID);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(
                nameof(SboServiceLayerConnectionEntity.Address), 
                nameof(SboServiceLayerConnectionEntity.Company))
            .IsUnique();
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.Company).IsRequired();
            entity.Property(e => e.User).IsRequired();
            entity.Property(e => e.Password).IsRequired();
        });

        modelBuilder.Entity<ConnectionEntity>(entity =>
        {
            entity.ToTable("INTCONNECTION");
            entity.ToTable(e => e.HasCheckConstraint("CK_INTCONNECTION_ALLOWEDCONTYPE", "ConnectionType IN ('MSSQL', 'HANA')"));

            entity.HasKey(e => e.ID);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
            entity.Property(e => e.ConnectionType).IsRequired();
            entity.Property(e => e.Host).IsRequired();
            entity.Property(e => e.Database).IsRequired();
            entity.Property(e => e.Port).IsRequired();
            entity.Property(e => e.DbUser).IsRequired();
            entity.Property(e => e.DbPassword).IsRequired();
        });

        modelBuilder.Entity<EventGeneratorEntity>(entity =>
        {
            entity
                .ToTable("INTEVENTGENERATOR")
                .ToTable(e => e.HasCheckConstraint("CK_EVENTGENERATOR_ALLOWEDTYPE", "Type IN ('Company', 'Timer', 'File')"));

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Active).HasDefaultValue(true).IsRequired();
            entity.HasOne(e => e.Manager).WithOne().HasForeignKey<EventGeneratorEntity>(e => e.ManagerID).IsRequired();
            entity.HasMany(e => e.CompanyEvents).WithOne(company => company.EventGenerator).HasForeignKey(ev => ev.EventGeneratorID).IsRequired();
            entity.HasOne(e => e.FileEvent).WithOne(file => file.EventGenerator).HasForeignKey<FileEventEntity>(ev => ev.EventGeneratorID).IsRequired();
        });

        modelBuilder.Entity<CompanyEventEntity>(entity =>
        {
            entity.ToTable("INTCOMPANYEVENT");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Active).HasDefaultValue(true).IsRequired();
            entity.Property(e => e.JobKey).IsRequired();
            entity.Property(e => e.CronExpression).HasDefaultValue("0 * * ? * *").IsRequired();
            entity.Property(e => e.SkipIfBusyStrategy).HasDefaultValue(false).IsRequired();
            entity.HasOne(e => e.Connection).WithOne().HasForeignKey<CompanyEventEntity>(e => e.ConnectionID).IsRequired();
        });

        modelBuilder.Entity<FileEventEntity>(entity =>
        {
            entity.ToTable("INTFILEEVENT");
            entity.ToTable(e => e.HasCheckConstraint("CK_FILEEVENT_TIMETOBEREADY", "TimeForFileToBeReady >= 5"));
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.JobKey).IsRequired();
            entity.Property(e => e.SkipIfBusyStrategy).HasDefaultValue(false).IsRequired();
            entity.Property(e => e.FileTypes).HasDefaultValue(".xlsx|.xls").IsRequired();
            entity.Property(e => e.TimeForFileToBeReady).HasDefaultValue(5).IsRequired();
            entity.Property(e => e.Directory).IsRequired();
        });

        modelBuilder.Entity<WorkerEntity>(entity =>
        {
            entity.ToTable("INTWORKER");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.ClassName).IsRequired();
            entity.Property(e => e.Active).HasDefaultValue(true).IsRequired();
            entity.HasMany(e => e.Parameters).WithOne().HasForeignKey(p => p.WorkerID);
            entity.HasOne<WorkerEntity>().WithOne().HasForeignKey<WorkerEntity>(e => e.PassWorker);
            entity.HasOne<WorkerEntity>().WithOne().HasForeignKey<WorkerEntity>(e => e.FailWorker);
        });

        modelBuilder.Entity<WorkerParameterEntity>(entity =>
        {
            entity.ToTable("INTWORKERPARAM");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Value).IsRequired();
        });

        modelBuilder.Entity<ManagerEntity>(entity =>
        {
            entity.ToTable("INTMANAGER");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.ClassName).IsRequired();
            entity.Property(e => e.FirstWorkerID).IsRequired();
            entity.Property(e => e.Active).HasDefaultValue(true).IsRequired();
            entity.HasMany(e => e.Parameters).WithOne().HasForeignKey(p => p.ManagerID);
            entity.HasOne(e => e.FirstWorker).WithOne().HasForeignKey<ManagerEntity>(m => m.FirstWorkerID).IsRequired();
        });

        modelBuilder.Entity<ManagerParameterEntity>(entity =>
        {
            entity.ToTable("INTMANAGERPARAM");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Value).IsRequired();
        });
    }

    private void CreateDbDirectoryIfNotExists(string dbPath)
    {
        FileInfo dbInfo = new FileInfo(dbPath);
        if (dbInfo.Directory?.Exists == false)
        {
            Directory.CreateDirectory(dbInfo.Directory.FullName);
        }
    }
}
