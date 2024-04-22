using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Exceptions;

public class ParameterCastException : IntegrationException
{
    public string ParameterName { get; private set; }

    public ParameterCastException(string message, string parameterName) : base(message)
    {
        ParameterName = parameterName;
    }
    public ParameterCastException(string message, string parameterName, Exception? innerException) 
        : base(message, innerException) 
    {
        ParameterName = parameterName;
    }
}
