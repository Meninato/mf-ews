using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers.NamedParameters;

public class CompanyWorkerNamedParameter : WorkerNamedParameter
{
    public CompanyWorkerNamedParameter(WorkerEntity worker) : base(worker) { }
}
