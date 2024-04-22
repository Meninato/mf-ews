using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers.NamedParameters;

public class ManagerNamedParameter
{
    public ManagerEntity Manager { get; private set; }
    public int EventGeneratorId { get; private set; }
    public bool SkipIfBusyStrategy { get; private set; }

    public ManagerNamedParameter(ManagerEntity manager, int eventGeneratorId, bool skipIfBusyStrategy)
    {
        Manager = manager;
        EventGeneratorId = eventGeneratorId;
        SkipIfBusyStrategy = skipIfBusyStrategy;
    }
}
