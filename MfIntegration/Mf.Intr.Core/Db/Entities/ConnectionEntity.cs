using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Db.Entities;
public class ConnectionEntity
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public IntrConnectionTypes ConnectionType { get; set; }
    public string? Host { get; set; }
    public string? Database { get; set; }
    public string? Schema { get; set; }
    public int? Port { get; set; }
    public string? DbUser { get; set; }
    public string? DbPassword { get; set; }
    public bool IsActive { get; set; }

}
