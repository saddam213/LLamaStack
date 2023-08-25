using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaStack.Core.Helpers
{
    public class JsonEncodingConverter : JsonConverter<Encoding>
    {
        public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Unexpected token type for Encoding.");

            var encodingName = reader.GetString();
            try
            {
                return Encoding.GetEncoding(encodingName);
            }
            catch
            {
                throw new JsonException($"Unsupported encoding: {encodingName}");
            }
        }

        public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.WebName);
        }
    }
}
