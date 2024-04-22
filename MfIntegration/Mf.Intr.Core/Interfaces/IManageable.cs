using Mf.Intr.Core.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;

public interface IManageable
{
    int Key { get; }
    string? Name { get; }
    int EventGeneratorKey { get; }
    bool SkipIfBusyStrategy { get; }
    bool IsBusy { get; }

    void StartWork();
    object? PrepareWorkMaterial();

    bool BeforeWorker(IWorkable workable);
    void AfterWorker(IWorkable workable, IWorkableResult workableResult);

    event EventHandler<WorkerEventArgs> OnWorkerFail;
    event EventHandler<WorkerEventArgs> OnWorkerSuccess;
}