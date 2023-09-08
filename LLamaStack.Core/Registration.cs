using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LLamaStack.Core
{
    public static class Registration
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
            serviceCollection.AddSingleton(ConfigManager.LoadConfiguration());
            serviceCollection.AddHostedService<ModelLoaderService<T>>();
            serviceCollection.AddSingleton<IModelService<T>, ModelService<T>>();
            serviceCollection.AddSingleton<IModelSessionService<T>, ModelSessionService<T>>();
            serviceCollection.AddSingleton<IModelSessionStateService<T>, ModelSessionStateService<T>>();
        }


        /// <summary>
        /// Register a custom IConfigSection section that is in the appsettings.json
        /// </summary>
        /// <typeparam name="T">The custom IConfigSection class type, NOTE: json section name MUST match class name</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        public static void AddLLamaCustomConfig<T>(this IServiceCollection serviceCollection) 
            where T : class, IConfigSection
        {
            serviceCollection.AddSingleton(ConfigManager.LoadConfiguration<T>());
        }
    }
}
