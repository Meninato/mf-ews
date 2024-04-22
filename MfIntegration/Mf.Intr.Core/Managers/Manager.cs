using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Workers;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Core.Managers;

public abstract class Manager : IManageable 
{
    private int _key;
    private string? _name;
    private int _eventGeneratorKey;
    private bool _skipIfBusyStrategy;
    private readonly IEnumerable<IWorkable> _workables;

    protected readonly ILogger _logger;
    protected readonly IManagerServiceBox _box;

    public int Key => _key;
    public string? Name => _name;
    public int EventGeneratorKey => _eventGeneratorKey;
    public bool SkipIfBusyStrategy => _skipIfBusyStrategy;
    public bool IsBusy { get; private set; }

    public event EventHandler<WorkerEventArgs>? OnWorkerFail;
    public event EventHandler<WorkerEventArgs>? OnWorkerSuccess;

    public Manager(IManagerServiceBox box)
    {
        _box = box;
        _box.Validate();

        _logger = _box.GetLogger();
        _workables = _box.GetWorkables();
    }

    public virtual object? PrepareWorkMaterial()
    {
        return null;
    }

    public virtual bool BeforeWorker(IWorkable workable) { return true; }

    public virtual void AfterWorker(IWorkable workable, IWorkableResult workableResult) { }

    public void StartWork()
    {
        try
        {
            if (ShouldSkipWork() == false)
            {
                IsBusy = true;
                Working();
            }
        }
        catch(Exception ex)
        {
            throw new ManagerException($"Manager [{Key}:{Name}] stopped his task because of an error [{ex.Message}]", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Working()
    {
        IWorkable? workable = _workables.FirstOrDefault(w => w.IsFirst);
        if (workable == null)
        {
            throw new KeyNotFoundException($"Unable to find the first Worker to start the task.");
        }

        object? material = PrepareWorkMaterial();
        if(material != null)
        {
            _logger.LogInformation("There is an input for the first worker: [{@input}]", material);
        }

        IWorkable previousWorkable = workable;
        IWorkableResult? workableResult;
        while (workable != null)
        {
            _logger.LogInformation("Calling worker [{key}:{name}]", workable.Key, workable.Name);

            //Reset worker state to clear any resources to avoid unexpected behavior
            //in case a worker in the chain call an already used worker
            workable.ResetState();

            //Additional step verification, you can perform an early check to allow or not the worker to continue
            if(BeforeWorker(workable))
            {
                try
                {
                    workableResult = workable.CallRun(material);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    workableResult = new WorkerResult<object>();
                    workableResult.Success = false;
                    workableResult.Message = ex.Message;
                }

                if (workableResult.Success)
                    OnWorkerSuccess?.Invoke(this, new WorkerEventArgs(workable, workableResult));
                else
                    OnWorkerFail?.Invoke(this, new WorkerEventArgs(workable, workableResult));

                previousWorkable = workable;
                workable = _workables.FirstOrDefault(workable => workable.Key == workableResult.Next);
                if (workable != null)
                {
                    material = workable.GetType().GetProperty(nameof(IWorkableResult<object>.Data))!.GetValue(workable, new object[] { });
                }
                else
                {
                    if(workableResult.Next.HasValue)
                    {
                        throw new IntegrationException($"Worker ID [{workableResult.Next.Value}] doens't exist.");
                    }
                }

                AfterWorker(previousWorkable, workableResult);
            }

            _logger.LogInformation("Worker [{key}:{name}] has finished his task", previousWorkable.Key, previousWorkable.Name);
        }

        _logger.LogInformation("Manager [{key}:{name}] has finished his task", Key, Name);
    }

    private bool ShouldSkipWork()
    {
        var shouldSkip = SkipIfBusyStrategy && IsBusy;
        if (shouldSkip)
        {
            _logger.LogWarning("Manager [{Key}:{Name}] is busy and has the property [SkipIfBusyStrategy=true], skipping right now.", Key, Name);
        }

        return shouldSkip;
    }

    private void InitManageablePrivateFields(int key, string? name, int eventGeneratorKey, bool skipIfBusyStrategy)
    {
        _key = key;
        _name = name;
        _eventGeneratorKey = eventGeneratorKey;
        _skipIfBusyStrategy = skipIfBusyStrategy;
    }
}
