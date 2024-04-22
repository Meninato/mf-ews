using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mf.Intr.Core.Attributes;

public class IntrColumnAttribute : Attribute
{
    public string Name { get; set; }

    public IntrColumnAttribute()
    {
        Name = string.Empty;
    }
}
