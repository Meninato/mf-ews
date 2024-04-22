using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Workers;
public sealed class TaskResult
{
    public object? Data { get; private set; }
    public bool HasError { get; private set; }
    public string? Message { get; private set; }

    public TaskResult(bool hasError, object? data = null, string? message = null)
    {
        HasError = hasError;
        Data = data;
        Message = message;
    }
}
