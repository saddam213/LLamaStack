using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaStack.Core.Converters
{
    public class JsonInterfaceConverter<TImplementation, TInterface> : JsonConverter<TInterface> where TImplementation : TInterface
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(TInterface);
        }

        public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TImplementation>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }
}
