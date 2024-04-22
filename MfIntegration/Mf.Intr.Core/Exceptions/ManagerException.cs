using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Exceptions;
public class ManagerException : IntegrationException
{
    public ManagerException()
    {
    }

    public ManagerException(string? message) : base(message)
    {
    }

    public ManagerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
