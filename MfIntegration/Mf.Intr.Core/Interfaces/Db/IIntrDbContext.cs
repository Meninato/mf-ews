using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Db;

public interface IIntrDbContext : IDisposable
{
    public DbConnection GetDbConnection();
    public void SetConnectionString(string connectionString);
}
