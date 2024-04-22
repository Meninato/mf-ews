using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;

namespace Mf.Intr.Core.Workers;

public abstract class Worker<TOutput, TInput> : Labor, IWorkable<TOutput, TInput> 
{
    public sealed override bool NeedsMaterialToWork => true;

    public Worker(IWorkerServiceBox box) : base(box)
    {

    }

    public abstract IWorkableResult<TOutput> Run(TInput indata);

    public sealed override IWorkableResult CallRun(object? data = null)
    {
        try
        {
            if (data == null)
            {
                throw new Exception($"Needs material to work.");
            }

            var result = Run((TInput)data);
            return result;
        }
        catch(Exception ex)
        {
            string errorMessage = ex.Message;
            if (ex is FileNotFoundException)
            {
                var fex = ex as FileNotFoundException;
                if (fex != null)
                {
                    errorMessage = string.IsNullOrEmpty(errorMessage) ? fex.FileName ?? "No information provided" : errorMessage;
                }
            }

            throw new WorkerException($"Worker [{Key}:{Name}] {errorMessage}", ex);
        }
    }
}

public abstract class Worker<TOutput> : Labor, IWorkable<TOutput>
{
    public sealed override bool NeedsMaterialToWork => false;

    public Worker(IWorkerServiceBox box) : base(box)
    {

    }

    public abstract IWorkableResult<TOutput> Run();

    public sealed override IWorkableResult CallRun(object? data = null)
    {
        try
        {
            var result = Run();
            return result;
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;
            if (ex is FileNotFoundException)
            {
                var fex = ex as FileNotFoundException;
                if (fex != null)
                {
                    errorMessage = string.IsNullOrEmpty(errorMessage) ? fex.FileName ?? "No information provided" : errorMessage;
                }
            }

            throw new WorkerException($"Worker [{Key}:{Name}] {errorMessage}", ex);
        }
    }
}
