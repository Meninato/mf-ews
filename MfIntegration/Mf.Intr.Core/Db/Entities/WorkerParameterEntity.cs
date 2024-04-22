using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;
public class WorkerParameterEntity
{
    public int ID { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Description { get; set; }

    public int WorkerID { get; set; }
}
