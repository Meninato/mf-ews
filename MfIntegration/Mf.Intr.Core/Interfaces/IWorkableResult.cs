using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;

public interface IWorkableResult
{
    bool Success { get; set; }
    int? Next { get; set; }
    string? Message { get; set; }
}

public interface IWorkableResult<T> : IWorkableResult
{
    T? Data { get; set; }
}
