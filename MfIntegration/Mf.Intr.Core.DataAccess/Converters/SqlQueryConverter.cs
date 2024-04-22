using Mf.Intr.Core.Attributes;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces.Db;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Converters;

public class SqlQueryConverter : QueryConverter, IDbQueryConverter
{
    public SqlQueryConverter() { }

    public string ConvertToInsert<T>(T obj) where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var tableName = GetTableName(obj);
        var columnsAndValues = GetColumnsAndValues(obj);
        var columns = columnsAndValues.Select(tuple => tuple.Item1).ToList();
        var values = columnsAndValues.Select(tuple => tuple.Item2).ToList();
        var keyColName = GetKeyColumnName(obj);

        var sql = $"INSERT INTO {tableName} ( {string.Join(", ", columns)} )";
        sql += keyColName != null ? $" OUTPUT INSERTED.{keyColName} " : string.Empty;
        sql += $" VALUES ( {string.Join(", ", values)} )";
        return sql;
    }

    public string ConvertToUpdate<T>(T obj) where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var keyColName = GetKeyColumnName(obj);
        if (keyColName == null)
        {
            throw new IntegrationException("To update an entity, you must define a key attribute first");
        }

        var keyProp = GetKeyPropertyInfo(obj);
        object? keyValue = keyProp!.GetValue(obj, null);
        if(keyValue == null || keyValue is string && string.IsNullOrEmpty(keyValue.ToString()))
        {
            throw new IntegrationException("To update an entity, you must set a value into key property.");
        }

        var tableName = GetTableName(obj);
        var columnsAndValues = GetColumnsAndValues(obj).ToList();

        var keyColAndValue = columnsAndValues.Single(tuple => tuple.Item1 == keyColName);
        columnsAndValues.Remove(keyColAndValue);
        var setClauses = columnsAndValues.Select(tuple => $"{tuple.Item1} = {tuple.Item2}").ToList();

        var sql = $"UPDATE {tableName} SET ";
        sql += string.Join(", ", setClauses);
        sql += $" WHERE {keyColAndValue.Item1} = {keyColAndValue.Item2} ";
        return sql;
    }
}
