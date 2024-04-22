using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mf.Intr.Application.Services.SboServiceLayer.Models;

namespace Mf.Intr.Application.Services.SboServiceLayer.Exceptions;

/// <summary>
/// Represents a Service Layer exception.
/// </summary>
public class SboServiceLayerException : Exception
{
    public SboServiceLayerErrorDetails ErrorDetails { get; set; }

    internal SboServiceLayerException(string? message, SboServiceLayerErrorDetails errorDetails, Exception innerException) : base(message, innerException)
    {
        ErrorDetails = errorDetails;
    }
}
