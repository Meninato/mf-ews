using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Exceptions;

public class WorkerException : IntegrationException
{
    public WorkerException()
    {
    }

    public WorkerException(string? message) : base(message)
    {
    }

    public WorkerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected WorkerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
