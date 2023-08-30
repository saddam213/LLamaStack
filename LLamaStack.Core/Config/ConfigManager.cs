using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaStack.Core.Config
{
    public class ConfigManager
    {

        /// <summary>
        /// Loads the LLamaStackConfig configuration object from appsetting.json
        /// </summary>
        /// <returns>LLamaStackConfig object</returns>
        public static LLamaStackConfig LoadConfiguration()
        {
            return LoadConfiguration<LLamaStackConfig>();
        }


        /// <summary>
        /// Loads a custom IConfigSection object from appsetting.json
        /// </summary>
        /// <typeparam name="T">The custom IConfigSection class type, NOTE: json section name MUST match class name</typeparam>
        /// <returns>The deserialized custom configuration object</returns>
        public static T LoadConfiguration<T>() where T : class, IConfigSection
        {
            return LoadConfigurationSection<T>();
        }


        private static T LoadConfigurationSection<T>() where T : class, IConfigSection
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
                var configElement = appsettingDocument.RootElement.GetProperty(typeof(T).Name);
                var configuration = configElement.Deserialize<T>(serializerOptions)
                    ?? throw new Exception($"Failed to parse {typeof(T).Name} json element");
                configuration.Initialize();
                return configuration;
            }
        }
    }
}
