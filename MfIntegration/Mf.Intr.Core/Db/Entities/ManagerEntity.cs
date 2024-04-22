using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;

public class ManagerEntity
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string ClassName { get; set; } = null!;
    public bool Active { get; set; }
    public int FirstWorkerID { get; set; }

    public List<ManagerParameterEntity>? Parameters { get; set; }
    public WorkerEntity FirstWorker { get; set; } = null!;
}
