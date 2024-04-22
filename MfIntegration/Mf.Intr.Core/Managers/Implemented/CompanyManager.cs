using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Managers.Implemented;

public abstract class CompanyManager : Manager, ICompanyManageable
{
    private string? _query;
    private ConnectionEntity _connection = null!;

    public string? Query => _query;
    public ConnectionEntity Connection => _connection;

    protected CompanyManager(IManagerServiceBox box) : base(box)
    {

    }

    private void InitCompanyManageablePrivateFields(string? query, ConnectionEntity connection)
    {
        _query = query;
        _connection = connection;
    }
}
