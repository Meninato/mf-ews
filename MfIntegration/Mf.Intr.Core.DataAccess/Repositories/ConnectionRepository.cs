using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Repositories;

public class ConnectionRepository : Repository<ConnectionEntity>, IConnectionRepository
{
    public ConnectionRepository(IntrDbContext context) : base(context)
    {
    }
}
