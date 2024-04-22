using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;

public class EventGeneratorEntity
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public EventGeneratorType Type { get; set; }
    public bool Active { get; set; }

    public int ManagerID { get; set; }
    public ManagerEntity Manager { get; set; } = null!;
    public List<CompanyEventEntity>? CompanyEvents { get; set; }
    public FileEventEntity? FileEvent { get; set; }

}
