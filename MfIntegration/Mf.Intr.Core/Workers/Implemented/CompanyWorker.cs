using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces;

namespace Mf.Intr.Core.Workers.Implemented;

public abstract class CompanyWorker<TOutput, TInput> : Worker<TOutput, TInput>, ICompanyWorkable
{
    private string? _query;
    private ConnectionEntity _connection = null!;

    public string? Query => _query;
    public ConnectionEntity Connection => _connection;

    protected CompanyWorker(IWorkerServiceBox box) : base(box) { }

    private void InitCompanyWorkablePrivateFields(string? query, ConnectionEntity connection)
    {
        _query = query;
        _connection = connection;
    }
}

public abstract class CompanyWorker<TOutput> : Worker<TOutput>, ICompanyWorkable
{
    private string? _query;
    private ConnectionEntity _connection = null!;

    public string? Query => _query;
    public ConnectionEntity Connection => _connection;

    protected CompanyWorker(IWorkerServiceBox box) : base(box)
    {
    }

    private void InitCompanyWorkablePrivateFields(string? query, ConnectionEntity connection)
    {
        _query = query;
        _connection = connection;
    }
}
