using LLamaStack.Core.Config;

namespace LLamaStack.Core.Services
{
    /// <summary>
    /// Service for managing language Models
    /// </summary>
    /// <typeparam name="T">Type used to identify contexts</typeparam>
    public interface IModelService<T> where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// Gets the model with the specified name.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        Task<LLamaStackModel<T>> GetModel(string modelName);


        /// <summary>
        /// Loads a model from a ModelConfig object.
        /// </summary>
        /// <param name="modelConfig">The model configuration.</param>
        Task<LLamaStackModel<T>> LoadModel(ModelConfig modelConfig);


        /// <summary>
        /// Loads all models found in appsettings.json
        /// </summary>
        Task LoadModels();


        /// <summary>
        /// Unloads the model with the specified name.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        Task UnloadModel(string modelName);


        /// <summary>
        /// Unloads all models.
        /// </summary>
        Task UnloadModels();


        /// <summary>
        /// Gets a context with the specified identifier
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The context identifier.</param>
        Task<LLamaStackContext> GetContext(string modelName, T contextId);


        /// <summary>
        /// Removes the context.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The context identifier.</param>
        Task<bool> RemoveContext(string modelName, T contextId);


        /// <summary>
        /// Creates a context.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The context identifier.</param>
        Task<LLamaStackContext> CreateContext(string modelName, T contextId);


        /// <summary>
        /// Gets the or create model and context.
        /// This will load a model from disk if not already loaded, and also create the context
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The context identifier.</param>
        /// <returns>Both loaded Model and Context</returns>
        Task<(LLamaStackModel<T>, LLamaStackContext)> GetOrCreateModelAndContext(string modelName, T contextId);
    }
}