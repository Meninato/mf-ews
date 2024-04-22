using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

public class SboServiceLayerPingResponse : ISboServiceLayerPingResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
    [JsonPropertyName("sender")]
    public string? Sender { get; set; }
    [JsonPropertyName("timestamp")]
    public decimal? Timestamp { get; set; }
    public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(Timestamp)).LocalDateTime;
    public bool IsSuccessStatusCode { get; internal set; }
    public HttpStatusCode StatusCode { get; internal set; }
}
