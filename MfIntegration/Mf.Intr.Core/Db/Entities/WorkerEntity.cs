using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;
public class WorkerEntity
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string ClassName { get; set; } = null!;
    public bool Active { get; set; }
    public int? PassWorker { get; set; }
    public int? FailWorker { get; set; }
    public List<WorkerParameterEntity>? Parameters { get; set; }
}
