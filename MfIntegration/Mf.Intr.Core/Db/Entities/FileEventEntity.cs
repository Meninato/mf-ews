using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;

public class FileEventEntity
{
    public int ID { get; set; }
    public int EventGeneratorID { get; set; }
    public string JobKey { get; set; } = null!;
    public bool SkipIfBusyStrategy { get; set; }
    public string FileTypes { get; set; } = null!;
    public int TimeForFileToBeReady { get; set; }
    public string Directory { get; set; } = null!;

    public EventGeneratorEntity EventGenerator { get; set; } = null!;
}
