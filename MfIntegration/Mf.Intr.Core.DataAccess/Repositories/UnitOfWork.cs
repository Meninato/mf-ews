using Mf.Intr.Core.DataAccess.Converters;
using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IntrDbContext _context;
    private readonly AppOptions _appOptions;
    private readonly ILogger _logger;
    private readonly IMsSqlServerDbContext _msSqlServerContext;
    private readonly IHanaDbContext _hanaContext;

    public IConnectionRepository ConnectionRepository { get; private set; }
    public ISboServiceLayerConnectionRepository SboServiceLayerConnectionRepository { get; private set; }
    public IWorkerRepository WorkerRepository { get; private set; }
    public IManagerRepository ManagerRepository { get; private set; }
    public IEventGeneratorRepository EventGeneratorRepository { get; private set; }

    public UnitOfWork(IntrDbContext context, 
        ILogger<UnitOfWork> logger, 
        IOptions<AppOptions> appSettingWrapper,
        IMsSqlServerDbContext msSqlServerContext,
        IHanaDbContext hanaContext)
    {
        _context = context;
        _appOptions = appSettingWrapper.Value;
        _logger = logger;
        _msSqlServerContext = msSqlServerContext;
        _hanaContext = hanaContext;

        ConnectionRepository = new ConnectionRepository(_context);
        SboServiceLayerConnectionRepository = new SboServiceLayerConnectionRepository(_context);
        WorkerRepository = new WorkerRepository(_context);
        ManagerRepository = new ManagerRepository(_context);
        EventGeneratorRepository = new EventGeneratorRepository(_context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public IIntrDataAccess GetDataAccess()
    {
        DbConnection connection = _context.Database.GetDbConnection();

        if (_appOptions.EnableEFCoreLogging)
        {
            connection = new LoggableDbConnection(connection, _logger);
        }

        return new IntrDataAccess(connection, new SqlQueryConverter());
    }

    public IIntrDataAccess GetDataAccess(ConnectionEntity connection)
    {
        string connectionString = ConnectionStringBuilder.BuildConnectionString(connection);
        return GetDataAccess(connection.ConnectionType, connectionString);
    }

    public IIntrDataAccess GetDataAccess(IntrConnectionTypes conTypes, string connectionString)
    {
        return conTypes switch
        {
            IntrConnectionTypes.MSSQL => PrepareDataAccess(_msSqlServerContext, connectionString, new SqlQueryConverter()),
            IntrConnectionTypes.HANA => PrepareDataAccess(_hanaContext, connectionString, new HanaQueryConverter()),
            _ => throw new IntegrationException("DbConnectionTypes was not found to get a db context")
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private IIntrDataAccess PrepareDataAccess(IIntrDbContext context, string? connectionString, IDbQueryConverter queryConverter)
    {
        if (string.IsNullOrEmpty(connectionString)) 
        { 
            throw new IntegrationException("ConnectionString cannot be empty"); 
        }

        context.SetConnectionString(connectionString);
        var connection =  context.GetDbConnection();

        if (_appOptions.EnableEFCoreLogging)
        {
            connection = new LoggableDbConnection(connection, _logger);
        }

        return new IntrDataAccess(connection, queryConverter);
    }
}
