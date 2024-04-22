using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Exceptions;

public class DatabaseCreationException : IntegrationException
{
    public DatabaseCreationException()
    {
    }

    public DatabaseCreationException(string? message) : base(message)
    {
    }

    public DatabaseCreationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DatabaseCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
