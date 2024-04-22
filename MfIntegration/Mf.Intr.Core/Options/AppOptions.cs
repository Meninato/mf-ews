using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Options;

public class AppOptions
{
    public bool EnableEFCoreLogging { get; set; }
    public LogOutputTypes LogOutputType { get; set; }
}
