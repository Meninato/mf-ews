using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Workers;

public class TaskEventArgs : EventArgs
{
    public TaskResult TaskResult { get; private set;}
    public object[]? Args { get; private set; }

    public TaskEventArgs(TaskResult taskResult, object[]? args)
    {
        TaskResult = taskResult;
        Args = args;
    }
}
