using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Converters;

internal class InterfaceConverter<TImplementation, TInterface> : JsonConverter<TInterface>
    where TImplementation : class, TInterface
{
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<TImplementation>(ref reader, options);

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {

    }
}

internal class InterfaceConverterFactory<TImplementation, TInterface> : JsonConverterFactory
{
    public Type ImplementationType { get; }
    public Type InterfaceType { get; }

    public InterfaceConverterFactory()
    {
        ImplementationType = typeof(TImplementation);
        InterfaceType = typeof(TInterface);
    }

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert == InterfaceType;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(InterfaceConverter<,>).MakeGenericType(ImplementationType, InterfaceType);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}
