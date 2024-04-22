using Mf.Intr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Workers;

public sealed class WorkerResult<T> : IWorkableResult<T> 
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public int? Next { get; set; }
    public string? Message { get; set; }
}
