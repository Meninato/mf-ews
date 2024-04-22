using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

public interface ISboServiceLayerPingResponse
{
    string Message { get; set; }
    string? Sender { get; set; }
    decimal? Timestamp { get; set; }
    DateTime DateTime { get; }
    bool IsSuccessStatusCode { get; }
    HttpStatusCode StatusCode { get; }
}
