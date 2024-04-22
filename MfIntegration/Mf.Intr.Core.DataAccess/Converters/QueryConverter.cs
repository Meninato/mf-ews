using Mf.Intr.Core.Attributes;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Converters;

public class QueryConverter
{
    public bool UseParameterInsteadValue = true;

    public PropertyInfo? GetKeyPropertyInfo(object instance)
    {
        var keyProps = instance.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(IntrColumnKeyAttribute))).ToList();

        if (keyProps.Count > 1)
        {
            throw new IntegrationException("Query Converter only supports one key");
        }

        return keyProps.FirstOrDefault();
    }

    protected string? GetKeyColumnName(object instance)
    {
        string? keyColName = null;
        var keyProp = GetKeyPropertyInfo(instance);
        if (keyProp != null)
        {
            keyColName = keyProp.Name;
            var keyColumnNameAttribute = keyProp.GetCustomAttribute(typeof(IntrColumnAttribute)) as IntrColumnAttribute;
            if (keyColumnNameAttribute != null)
            {
                keyColName = keyColumnNameAttribute.Name;
            }

            keyColName = AddBrackets(keyColName);
        }

        return keyColName;
    }

    protected IntrColumnKeyAttribute? GetColumnKeyAttribute(object instance)
    {
        return instance.GetType().GetCustomAttribute(typeof(IntrColumnKeyAttribute)) as IntrColumnKeyAttribute;
    }

    protected IntrTableAttribute GetTableAttribute(object instance)
    {
        IntrTableAttribute? tableAttribute = instance.GetType().GetCustomAttribute(typeof(IntrTableAttribute)) as IntrTableAttribute;
        if (tableAttribute == null)
        {
            throw new IntegrationException("Missing attribute TableAttribute to define the table name");
        }

        return tableAttribute;
    }

    protected string GetTableName(object instance)
    {
        var tableAttribute = GetTableAttribute(instance);
        string tableName = AddBrackets(tableAttribute.Name);

        return tableName;
    }

    protected IEnumerable<Tuple<string, string>> GetColumnsAndValues(object instance)
    {
        var colAndValues = new List<Tuple<string, string>>();

        foreach (var propertyInfo in instance.GetType().GetProperties())
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (propertyInfo.PropertyType.IsValueType && underlyingType == null)
            {
                throw new IntegrationException($"Object property {propertyInfo.Name} is a value type and must implement Nullable.");
            }

            string colName = GetColumnName(propertyInfo);

            string? colValue = null;
            if (UseParameterInsteadValue)
            {
                var objValue = propertyInfo.GetValue(instance, null);
                if (objValue != null)
                {
                    colValue = $"@{propertyInfo.Name}";
                }
            }
            else
            {
                colValue = GetColumnValue(propertyInfo, instance, underlyingType!);
            }

            if (colValue != null)
            {
                colAndValues.Add(new Tuple<string, string>(colName, colValue));
            }
        }

        return colAndValues;
    }

    protected string GetColumnName(PropertyInfo propertyInfo)
    {
        string colName = propertyInfo.Name;
        var columnAttribute = propertyInfo.GetCustomAttribute(typeof(IntrColumnAttribute)) as IntrColumnAttribute;
        if (columnAttribute != null)
        {
            colName = columnAttribute.Name;
        }

        colName = AddBrackets(colName);

        return colName;
    }

    protected string AddBrackets(string colName)
    {
        if (colName.StartsWith("[") == false) { colName = $"[{colName}"; }
        if (colName.EndsWith("]") == false) { colName = $"{colName}]"; }

        return colName;
    }

    protected string? GetColumnValue(PropertyInfo propertyInfo, object instance, Type underlyingType)
    {
        string? colValue = null;
        object? objValue = propertyInfo.GetValue(instance, null);
        string underlyingTypeName = propertyInfo.PropertyType.IsValueType ? underlyingType.Name : propertyInfo.PropertyType.Name;

        if (objValue != null)
        {
            if (underlyingTypeName == "String")
            {
                colValue = GetStringValue(objValue);
            }
            else if (underlyingTypeName == "Boolean")
            {
                colValue = GetBooleanValue(objValue);
            }
            else if (underlyingTypeName == "DateTime")
            {
                colValue = GetDateTimeValue(objValue);
            }
            else if (underlyingTypeName == "Decimal")
            {
                colValue = GetDecimalValue(objValue);
            }
            else if (underlyingTypeName == "Double")
            {
                colValue = GetDoubleValue(objValue);
            }
            else if (underlyingTypeName == "Single")
            {
                colValue = GetSingleValue(objValue);
            }
            else
            {
                colValue = objValue.ToString();
            }
        }

        return colValue;
    }

    protected virtual string GetStringValue(object value)
    {
        return "'" + value.ToString() + "'";
    }

    protected virtual string GetBooleanValue(object value)
    {
        int v = Convert.ToInt32(value);
        return v.ToString();
    }

    protected virtual string GetDateTimeValue(object value)
    {
        var v = (DateTime)value;
        return "'" + v.ToString("yyyy-MM-dd HH:mm:ss") + "'";
    }

    protected virtual string GetDecimalValue(object value)
    {
        return value.ToString()!;
    }

    protected virtual string GetDoubleValue(object value)
    {
        return value.ToString()!;
    }

    protected virtual string GetSingleValue(object value)
    {
        return value.ToString()!;
    }

    protected virtual string GetUnMappedValue(object value)
    {
        return value.ToString()!;
    }
}
