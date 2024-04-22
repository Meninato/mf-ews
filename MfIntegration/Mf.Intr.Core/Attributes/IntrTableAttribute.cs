using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Attributes;

public class IntrTableAttribute : Attribute
{
    public string Name { get; set; }

    public IntrTableAttribute()
    {
        Name = string.Empty;
    }
}
