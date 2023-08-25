using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace LLamaStack.Core.Config
{
    public class ConfigManager
    {
        public static LLamaStackConfig LoadConfiguration()
        {
            var appsettingStreamFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(appsettingStreamFile))
                throw new FileNotFoundException(appsettingStreamFile);

            using var appsettingStream = File.OpenRead(appsettingStreamFile);
            {
                var serializerOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                var appsettingDocument = JsonSerializer.Deserialize<JsonDocument>(appsettingStream, serializerOptions)
                    ?? throw new Exception("Failed to parse appsetting document");
                var configElement = appsettingDocument.RootElement.GetProperty(nameof(LLamaStackConfig));
                var configuration = configElement.Deserialize<LLamaStackConfig>(serializerOptions)
                    ?? throw new Exception($"Failed to parse {nameof(LLamaStackConfig)} json element");
                configuration.Initialize();
                return configuration;
            }
        }
    }
}
