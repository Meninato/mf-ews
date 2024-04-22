using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers;

public static class ParameterCaster
{
    public static T CastParameterValue<T>(string value) where T : IConvertible
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static object CastParameterValue(string primitiveName, string value)
    {
        object newValue = primitiveName switch
        {
            "string" => CastParameterValue<string>(value),
            "int" => CastParameterValue<int>(value),
            "double" => CastParameterValue<double>(value),
            "float" => CastParameterValue<float>(value),
            "decimal" => CastParameterValue<decimal>(value),
            "bool" => CastParameterValue<bool>(value),
            _ => throw new IntegrationException($"Parameter type {primitiveName} not supported.")
        };

        return newValue;
    }
}
