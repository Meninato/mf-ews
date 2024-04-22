using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers.NamedParameters;

public class CompanyManagerNamedParameter : ManagerNamedParameter
{
    public string? Query { get; private set; }
    public ConnectionEntity Connection { get; private set; }

    public CompanyManagerNamedParameter(ManagerEntity manager, int eventGeneratorId, bool skipIfBusyStrategy, 
        ConnectionEntity connection, string? query) : base(manager, eventGeneratorId, skipIfBusyStrategy)
    {
        Connection = connection;
        Query = query;
    }
}
