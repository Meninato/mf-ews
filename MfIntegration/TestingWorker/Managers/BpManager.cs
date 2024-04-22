using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Managers.Implemented;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingWorker.Managers;

public class BpManager : CompanyManager
{
    public BpManager(IManagerServiceBox box) : base(box)
    {

    }

    public override object? PrepareWorkMaterial()
    {
        return "lets go code";
    }
}
