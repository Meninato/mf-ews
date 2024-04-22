using Mf.Intr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Workers;

public class WorkerEventArgs : EventArgs
{
    public IWorkable Workable { get; private set; }
    public IWorkableResult WorkableResult { get; private set; }

    public WorkerEventArgs(IWorkable workable, IWorkableResult workableResult)
    {
        Workable = workable;
        WorkableResult = workableResult;
    }
}
