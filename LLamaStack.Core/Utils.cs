using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaStack.Core
{
    public static class Utils
    {

        /// <summary>
        /// Register LLamaStack services
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public static void AddLLamaStack(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLLamaStack<Guid>();
        }


        /// <summary>
        /// Register LLamaStack services
        /// </summary>
        /// <typeparam name="T">The type used for session identification</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        public static void AddLLamaStack<T>(this IServiceCollection serviceCollection) 
            where T : IEquatable<T>, IComparable<T>
        {
            serviceCollection.AddSingleton(ReadAppSettings());
            serviceCollection.AddHostedService<ModelLoaderService>();
            serviceCollection.AddSingleton<IModelService, ModelService>();
            serviceCollection.AddSingleton<IModelSessionService<T>, ModelSessionService<T>>();
            serviceCollection.AddSingleton<IModelSessionStateService<T>, ModelSessionStateService<T>>();
        }

        private static LLamaStackConfig ReadAppSettings()
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
