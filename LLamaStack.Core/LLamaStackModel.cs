using LLama;
using LLama.Abstractions;
using LLamaStack.Core.Config;
using System.Collections.Concurrent;
using System.Text;

namespace LLamaStack.Core
{
    public class LLamaStackModel : IDisposable
    {
        private readonly ModelConfig _config;
        private readonly LLamaWeights _weights;
        private readonly ConcurrentDictionary<string, LLamaStackModelContext> _contexts;

        public LLamaStackModel(ModelConfig modelParams)
        {
            _config = modelParams;
            _weights = LLamaWeights.LoadFromFile(modelParams);
            _contexts = new ConcurrentDictionary<string, LLamaStackModelContext>();
        }

        public int ContextCount => _contexts.Count;

        /// <summary>
        /// Creates a new context session on this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>LLamaModelContext for this LLamaModel</returns>
        /// <exception cref="Exception">Context exists</exception>
        public Task<LLamaStackModelContext> CreateContext(string contextId)
        {
            if (_contexts.TryGetValue(contextId, out var context))
                throw new Exception($"Context with id {contextId} already exists.");

            if (_config.MaxInstances > -1 && ContextCount >= _config.MaxInstances)
                throw new Exception($"Maximum model instances reached");

            context = new LLamaStackModelContext(_weights.CreateContext(_config));
            if (_contexts.TryAdd(contextId, context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaStackModelContext>(null);
        }

        /// <summary>
        /// Get a contexts belonging to this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>LLamaModelContext for this LLamaModel with the specified contextId</returns>
        public Task<LLamaStackModelContext> GetContext(string contextId)
        {
            if (_contexts.TryGetValue(contextId, out var context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaStackModelContext>(null);
        }

        /// <summary>
        /// Remove a context from this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>true if removed, otherwise false</returns>
        public Task<bool> RemoveContext(string contextId)
        {
            if (!_contexts.TryRemove(contextId, out var context))
                return Task.FromResult(false);

            context?.Dispose();
            return Task.FromResult(true);
        }


        /// <inheritdoc />
        public virtual void Dispose()
        {
            foreach (var context in _contexts.Values)
            {
                context?.Dispose();
            }
            _weights.Dispose();
        }
    }
}

