using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

public class SboServiceLayerLoginResponse : ISboServiceLayerLoginResponse
{
    public string SessionId { get; set; } = null!;
    public string Version { get; set; } = null!;
    public int SessionTimeout { get; set; }
    public DateTime LastLogin { get; set; }

    public SboServiceLayerLoginResponse() { }

    public SboServiceLayerLoginResponse(string sessionId, string version, int sessionTimeout, DateTime lastLogin)
    {
        SessionId = sessionId;
        Version = version;
        SessionTimeout = sessionTimeout;
        LastLogin = lastLogin;
    }
}
