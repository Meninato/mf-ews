using Mf.Intr.Application.Services.SboServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Converters;

internal class SboServiceLayerErrorResponseJsonConverter : JsonConverter<SboServiceLayerErrorResponse>
{
    public override SboServiceLayerErrorResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDoc = JsonDocument.ParseValue(ref reader);
        var slError = new SboServiceLayerErrorResponse();

        var errorJsonEl = jsonDoc.RootElement.GetProperty("error");
        var hasCodeProperty = errorJsonEl.TryGetProperty("code", out JsonElement codeJsonEl);
        var hasMessageProperty = errorJsonEl.TryGetProperty("message", out JsonElement messageJsonEl);
        if (hasCodeProperty)
        {
            slError.Error.Code = codeJsonEl.ToString();
        }
        if (hasMessageProperty)
        {
            if (messageJsonEl.ValueKind == JsonValueKind.Object)
            {
                var hasValueProperty = messageJsonEl.TryGetProperty("value", out JsonElement valueJsonEl);
                var hasLangProperty = messageJsonEl.TryGetProperty("lang", out JsonElement langJsonEl);
                if (hasValueProperty)
                {
                    slError.Error.Message.Value = valueJsonEl.GetString();
                }
                if (hasLangProperty)
                {
                    slError.Error.Message.Lang = langJsonEl.GetString();
                }
            }
            else if (messageJsonEl.ValueKind == JsonValueKind.String)
            {
                slError.Error.Message.Value = messageJsonEl.GetString();
            }
        }

        return slError;
    }

    public override void Write(Utf8JsonWriter writer, SboServiceLayerErrorResponse value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
