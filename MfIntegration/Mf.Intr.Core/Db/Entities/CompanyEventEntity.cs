using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;

public class CompanyEventEntity
{
    public int ID { get; set; }
    public int ConnectionID { get; set; }
    public bool Active { get; set; }
    public string CronExpression { get; set; } = null!;
    public int EventGeneratorID { get; set; }
    public string? Query { get; set; }
    public string JobKey { get; set; } = null!;
    public bool SkipIfBusyStrategy { get; set; }

    public ConnectionEntity Connection { get; set; } = null!;
    public EventGeneratorEntity EventGenerator { get; set; } = null!;

}
