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
public class LoggableDbCommand : DbCommand
{
    private readonly DbCommand _command;
    private readonly ILogger _logger;
    private bool _designTimeVisible = false;

    [AllowNull]
    public override string CommandText { get => _command.CommandText; set => _command.CommandText = value; }
    public override int CommandTimeout { get => _command.CommandTimeout; set => _command.CommandTimeout = value; }
    public override CommandType CommandType { get => _command.CommandType; set => _command.CommandType = value; }
    public override bool DesignTimeVisible { get => _designTimeVisible; set => _designTimeVisible = value; }
    public override UpdateRowSource UpdatedRowSource { get => _command.UpdatedRowSource; set => _command.UpdatedRowSource = value; }
    protected override DbConnection? DbConnection { get => _command.Connection; set => _command.Connection = value; }
    protected override DbParameterCollection DbParameterCollection => _command.Parameters;
    protected override DbTransaction? DbTransaction { get => _command.Transaction; set => _command.Transaction = value; }

    public LoggableDbCommand(DbCommand command, ILogger logger)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _command = command;
        _logger = logger;
    }

    public override void Cancel()
    {
        _command.Cancel();
    }

    public override int ExecuteNonQuery()
    {
        Dump();
        return _command.ExecuteNonQuery();
    }

    public override object? ExecuteScalar()
    {
        Dump();
        return _command.ExecuteScalar();
    }

    public override void Prepare()
    {
        _command.Prepare();
    }

    protected override DbParameter CreateDbParameter()
    {
        return _command.CreateParameter();
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        Dump();
        return _command.ExecuteReader(behavior);
    }

    private void Dump()
    {
        _logger.LogInformation("SQL COMMAND: {commandText}", _command.CommandText);
        for (int i = 0; i < _command.Parameters.Count; i++)
        {
            DbParameter dbParam = _command.Parameters[i];
            if (dbParam != null)
            {
                _logger.LogInformation("SQL PARAMETER Name: {name}, Value: {value}", dbParam.ParameterName, dbParam.Value);
            }
        }
    }
}
