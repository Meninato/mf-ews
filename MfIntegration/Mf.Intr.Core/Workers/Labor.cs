using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Core.Workers;
public abstract class Labor : IWorkable
{
    private readonly List<TaskResult> _tasksResult;
    private int _key;
    private string? _name;
    private bool _isFirst;
    private int? _next;
    private int? _failure;

    protected readonly ILogger _logger;
    protected readonly IWorkerServiceBox _box;

    public int Key => _key;
    public string? Name => _name;
    public bool IsFirst => _isFirst;
    public int? Next => _next;
    public int? Failure => _failure;

    public abstract bool NeedsMaterialToWork { get; }

    public event EventHandler<TaskEventArgs>? OnTaskFail;
    public event EventHandler<TaskEventArgs>? OnTaskSuccess;

    public IReadOnlyCollection<TaskResult> TaskResults => _tasksResult.AsReadOnly();
    public IReadOnlyCollection<TaskResult> SuccessTaskResults => _tasksResult.Where(task => task.HasError == false).ToList().AsReadOnly();
    public IReadOnlyCollection<TaskResult> FailureTaskResults => _tasksResult.Where(task => task.HasError).ToList().AsReadOnly();

    public Labor(IWorkerServiceBox box)
    {
        _box = box;
        _box.Validate();

        _logger = _box.GetLogger();
        _tasksResult = new List<TaskResult>();
    }

    public abstract IWorkableResult CallRun(object? data = null);

    public virtual void ResetState()
    {
        _tasksResult.Clear();
    }

    public bool IsWorkableOf<T>(IWorkable workable) where T: IWorkable
    {
        var typeToCheck = typeof(T);
        var workablesInterfaces = new List<Type> { typeof(ISharedWorkable), typeof(IFileWorkable), typeof(ICompanyWorkable) };
        if (typeToCheck.IsInterface == false && workablesInterfaces.Any(mi => mi.FullName == typeToCheck.FullName) == false)
        {
            throw new IntegrationException("The type T must be an implemented interface of IWorkable. E.g: ISharedWorkable");
        }

        return workable.GetType().GetInterfaces().Any(i => i.IsGenericType == false 
            && i.FullName == typeToCheck.FullName);
    }

    public WorkStatus GetWorkStatus()
    {
        WorkStatus workStatus = WorkStatus.NotStarted;
        if (_tasksResult.Count > 0)
        {
            if (_tasksResult.All(result => result.HasError == false))
            {
                workStatus = WorkStatus.AllSucceed;
            }
            else if (_tasksResult.All(result => result.HasError))
            {
                workStatus = WorkStatus.AllFailed;
            }
            else
            {
                workStatus = WorkStatus.Partial;
            }
        }

        return workStatus;
    }

    public TaskResult StartTask(RunWorkerTask func, bool append = true, bool throwOnError = false)
    {
        return CallStartTask(func, null, append);
    }

    public TaskResult StartTask<T1>(RunWorkerTask<T1> func, object[] args, bool append = true, bool throwOnError = false)
    {
        return CallStartTask(func, args, append);
    }

    public TaskResult StartTask<T1, T2>(RunWorkerTask<T1, T2> func, object[] args, bool append = true, bool throwOnError = false)
    {
        return CallStartTask(func, args, append);
    }

    public TaskResult StartTask<T1, T2, T3>(RunWorkerTask<T1, T2, T3> func, object[] args, bool append = true, bool throwOnError = false)
    {
        return CallStartTask(func, args, append);
    }

    private TaskResult CallStartTask(Delegate func, object[]? args, bool append, bool throwOnError = false)
    {
        if(args != null && args.Length > 0)
        {
            _logger.LogInformation("Worker [{key}:{name}] is running a task with the defined input [{@input}]", Key, Name, args);
        }
        else
        {
            _logger.LogInformation("Worker [{key}:{name}] is running a task.", Key, Name);
        }

        TaskResult? taskResult;
        try
        {
            taskResult = func.DynamicInvoke(args) as TaskResult;
            if (taskResult == null)
                throw new Exception("TaskResult is null");
        }
        catch (Exception ex)
        {
            taskResult = new TaskResult(true, args, ex.Message);
            if(throwOnError)
            {
                throw;
            }
        }

        if (append)
        {
            _tasksResult.Add(taskResult);
        }

        if (taskResult.HasError)
            OnTaskFail?.Invoke(this, new TaskEventArgs(taskResult, args));
        else
            OnTaskSuccess?.Invoke(this, new TaskEventArgs(taskResult, args));

        return taskResult;
    }

    private void InitWorkablePrivateFields(int key, string? name, int? next, int? failure, bool isFirst)
    {
        _key = key;
        _name = name;
        _isFirst = isFirst;
        _next = next;
        _failure = failure;
    }
}
