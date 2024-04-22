using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces.Db;

namespace Mf.Intr.Core.DataAccess;

public class IntrDataAccess : IIntrDataAccess
{
    private readonly DbConnection _wrappedConnection;
    private readonly IDbQueryConverter _queryConverter;

    public DbConnection Connection => _wrappedConnection;

    public IntrDataAccess(DbConnection connection, IDbQueryConverter queryConverter)
    {
        _wrappedConnection = connection;
        _queryConverter = queryConverter;
    }

    public TKey? Insert<TEntity,TKey>(TEntity entity) where TEntity : class
    {
        string parameterizedSql = _queryConverter.ConvertToInsert(entity);
        var key = _wrappedConnection.Query<TKey>(parameterizedSql, param: entity).SingleOrDefault();
        var keyProp = _queryConverter.GetKeyPropertyInfo(entity);
        if(keyProp != null)
        {
            var changeType = keyProp.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(keyProp.PropertyType);
            if(keyProp.PropertyType.IsValueType && underlyingType != null)
            {
                changeType = underlyingType;
            }

            keyProp.SetValue(entity, Convert.ChangeType(key, changeType), null);
        }

        return key;
    }

    public bool Update<TEntity>(TEntity entity) where TEntity : class
    {
        string parameterized = _queryConverter.ConvertToUpdate(entity);
        int rowsAffected = _wrappedConnection.Execute(parameterized, entity);

        return rowsAffected > 0;
    }

    public IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.Execute(sql, param, transaction, commandTimeout, commandType);
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _wrappedConnection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }
}
