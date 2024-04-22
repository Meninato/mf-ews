using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

public interface ISboServiceLayerLoginResponse
{
    string SessionId { get; set; }
    string Version { get; set; }
    int SessionTimeout { get; set; }
    DateTime LastLogin { get; set; }
}
