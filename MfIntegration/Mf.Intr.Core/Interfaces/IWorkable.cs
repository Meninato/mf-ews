using Mf.Intr.Core.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;

public delegate TaskResult RunWorkerTask();
public delegate TaskResult RunWorkerTask<T1>(T1 param1);
public delegate TaskResult RunWorkerTask<T1, T2>(T1 param1, T2 param2);
public delegate TaskResult RunWorkerTask<T1, T2, T3>(T1 param1, T2 param2, T3 param3);

public interface IWorkable
{
    int Key { get; }
    string? Name { get; }
    bool IsFirst { get; }
    bool NeedsMaterialToWork { get; }
    int? Next { get; }
    int? Failure { get; }

    void ResetState();
    WorkStatus GetWorkStatus(); 
    IReadOnlyCollection<TaskResult> TaskResults { get; }
    IReadOnlyCollection<TaskResult> SuccessTaskResults { get; }
    IReadOnlyCollection<TaskResult> FailureTaskResults { get; }
    TaskResult StartTask(RunWorkerTask func, bool append = true, bool throwOnError = false);
    TaskResult StartTask<T1>(RunWorkerTask<T1> func, object[] args, bool append = true, bool throwOnError = false);
    TaskResult StartTask<T1, T2>(RunWorkerTask<T1, T2> func, object[] args, bool append = true, bool throwOnError = false);
    TaskResult StartTask<T1, T2, T3>(RunWorkerTask<T1, T2, T3> func, object[] args, bool append = true, bool throwOnError = false);
    IWorkableResult CallRun(object? input = null);

    bool IsWorkableOf<T>(IWorkable workable) where T : IWorkable;

    event EventHandler<TaskEventArgs> OnTaskFail;
    event EventHandler<TaskEventArgs> OnTaskSuccess;
}

public interface IWorkable<TOutput> : IWorkable
{
    IWorkableResult<TOutput> Run();
}

public interface IWorkable<TOutput, TInput> : IWorkable 
{
    IWorkableResult<TOutput> Run(TInput indata);
}
