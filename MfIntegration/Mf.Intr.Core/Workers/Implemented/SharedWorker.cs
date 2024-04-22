using Mf.Intr.Core.Interfaces;

namespace Mf.Intr.Core.Workers.Implemented;

public abstract class SharedWorker<TOutput, TInput> : Worker<TOutput, TInput>, ISharedWorkable
{
    protected SharedWorker(IWorkerServiceBox box) : base(box)
    {
    }
}

public abstract class SharedWorker<TOutput> : Worker<TOutput>, ISharedWorkable
{
    protected SharedWorker(IWorkerServiceBox box) : base(box)
    {
    }
}
