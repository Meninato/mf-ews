using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess;

[DesignerCategory("")]
public class LoggableDbConnection : DbConnection
{
    private readonly DbConnection _conn;
    private readonly ILogger _logger;

    [AllowNull]
    public override string ConnectionString { get => _conn.ConnectionString; set => _conn.ConnectionString = value; }

    public override string Database => _conn.Database;

    public override string DataSource => _conn.DataSource;

    public override string ServerVersion => _conn.ServerVersion;

    public override ConnectionState State => _conn.State;

    public LoggableDbConnection(DbConnection connection, ILogger logger)
    {
        if(connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if(logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _conn = connection;
        _logger = logger;
    }

    public override void ChangeDatabase(string databaseName)
    {
        _conn.ChangeDatabase(databaseName);
    }

    public override void Close()
    {
        _conn.Close();
    }

    public override void Open()
    {
        _conn.Open();
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return _conn.BeginTransaction(isolationLevel);
    }

    protected override DbCommand CreateDbCommand()
    {
        DbCommand underlyingCommand = _conn.CreateCommand();
        return new LoggableDbCommand(underlyingCommand, _logger);
    }
}
