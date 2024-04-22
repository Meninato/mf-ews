using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Db;
public interface IUnitOfWork : IDisposable
{
    IConnectionRepository ConnectionRepository { get; }
    ISboServiceLayerConnectionRepository SboServiceLayerConnectionRepository { get; }
    IWorkerRepository WorkerRepository { get; }
    IManagerRepository ManagerRepository { get; }
    IEventGeneratorRepository EventGeneratorRepository { get; }
    Task SaveChangesAsync();

    /// <summary>
    /// Get integration SQLite connection.
    /// Don't dispose the connection it is auto managed.
    /// </summary>
    /// <returns>Return a wrapper under dbconnection and dapper of SQLite integration database</returns>
    IIntrDataAccess GetDataAccess();

    /// <summary>
    /// Get external connection using connectionString
    /// Don't dispose the connection it is auto managed.
    /// </summary>
    /// <param name="conTypes">Connection type</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Return a wrapper under dbconnection and dapper of selected database</returns>
    IIntrDataAccess GetDataAccess(IntrConnectionTypes conTypes, string connectionString);

    /// <summary>
    /// Get external connection using ConnectionEntity object.
    /// Don't dispose the connection it is auto managed.
    /// </summary>
    /// <param name="connection">ConnectionEntity object</param>
    /// <returns>Return a wrapper under dbconnection and dapper of selected database</returns>
    IIntrDataAccess GetDataAccess(ConnectionEntity connection);
}
