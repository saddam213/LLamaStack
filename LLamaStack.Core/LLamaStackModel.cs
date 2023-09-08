using LLama;
using LLama.Abstractions;
using LLamaStack.Core.Config;
using LLamaStack.Core.Extensions;
using System.Collections.Concurrent;

namespace LLamaStack.Core
{
    /// <summary>
    /// Wrapper class for LLamaSharp LLamaWeights
    /// </summary>
    /// <typeparam name="T">Type used to identify contexts</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class LLamaStackModel<T> : IDisposable
    {
        private readonly ModelConfig _config;
        private readonly LLamaWeights _weights;
        private readonly ConcurrentDictionary<T, LLamaStackContext> _contexts;

        public LLamaStackModel(ModelConfig modelParams)
        {
            _config = modelParams;
            _weights = LLamaWeights.LoadFromFile(modelParams.ToModelParams());
            _contexts = new ConcurrentDictionary<T, LLamaStackContext>();
        }

        /// <summary>
        /// Gets the model configuration.
        /// </summary>
        public IModelParams ModelParams => _config.ToModelParams();

        /// <summary>
        /// Gets the LLamaWeights
        /// </summary>
        public LLamaWeights LLamaWeights => _weights;


        /// <summary>
        /// Gets the context count.
        /// </summary>
        public int ContextCount => _contexts.Count;


        /// <summary>
        /// Creates a new context session on this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>LLamaModelContext for this LLamaModel</returns>
        /// <exception cref="Exception">Context exists</exception>
        public Task<LLamaStackContext> CreateContext(T contextId)
        {
            if (_contexts.TryGetValue(contextId, out var context))
                throw new Exception($"Context with id {contextId} already exists.");

            if (_config.MaxInstances > -1 && ContextCount >= _config.MaxInstances)
                throw new Exception($"Maximum model instances reached");

            context = new LLamaStackContext(_weights.CreateContext(_config.ToModelParams()));
            if (_contexts.TryAdd(contextId, context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaStackContext>(null);
        }

        /// <summary>
        /// Get a contexts belonging to this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>LLamaModelContext for this LLamaModel with the specified contextId</returns>
        public Task<LLamaStackContext> GetContext(T contextId)
        {
            if (_contexts.TryGetValue(contextId, out var context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaStackContext>(null);
        }

        /// <summary>
        /// Remove a context from this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>true if removed, otherwise false</returns>
        public Task<bool> RemoveContext(T contextId)
        {
            if (!_contexts.TryRemove(contextId, out var context))
                return Task.FromResult(false);

            context?.Dispose();
            return Task.FromResult(true);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var context in _contexts.Values)
            {
                context?.Dispose();
            }
            _weights.Dispose();
        }
    }
}

