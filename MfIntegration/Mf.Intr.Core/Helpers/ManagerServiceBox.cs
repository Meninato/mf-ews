using Mf.Intr.Core.Exceptions;
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

namespace Mf.Intr.Core.Helpers;

public class ManagerServiceBox : ServiceBox, IManagerServiceBox
{
    public override void Validate()
    {
        base.Validate();
        if (this.Contains<IEnumerable<IWorkable>>() == false) throw new IntegrationException("Missing IEnumerable<IWorkable>> for ManagerServiceBox");
    }

    public IEnumerable<IWorkable> GetWorkables()
    {
        return this.Get<IEnumerable<IWorkable>>();
    }
}
