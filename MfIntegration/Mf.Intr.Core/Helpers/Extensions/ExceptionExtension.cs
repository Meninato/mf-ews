using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Helpers.Extensions;
public static class ExceptionExtension
{
    public static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
    {
        if (ex == null)
        {
            throw new IntegrationException("ex");
        }

        var innerException = ex;
        do
        {
            yield return innerException;
            innerException = innerException.InnerException;
        }
        while (innerException != null);
    }
}
