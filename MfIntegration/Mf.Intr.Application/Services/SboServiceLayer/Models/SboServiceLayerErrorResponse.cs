using Mf.Intr.Application.Services.SboServiceLayer.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

[JsonConverter(typeof(SboServiceLayerErrorResponseJsonConverter))]
public class SboServiceLayerErrorResponse
{
    [JsonPropertyName("error")]
    public SboServiceLayerErrorDetails Error { get; set; } = new SboServiceLayerErrorDetails();
}

public class SboServiceLayerErrorDetails
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("message")]
    public SboServiceLayerErrorMessage Message { get; set; } = new SboServiceLayerErrorMessage();
}

public class SboServiceLayerErrorMessage
{
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}