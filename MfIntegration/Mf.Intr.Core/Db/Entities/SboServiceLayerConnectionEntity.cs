using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;

public class SboServiceLayerConnectionEntity
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Company { get; set; } = null!;
    public int? Language { get; set; }
    public int? NumberOfAttempts { get; set; }
}
