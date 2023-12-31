﻿using LLamaStack.Core.Async;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using System.Collections.Concurrent;

namespace LLamaStack.Core.Services
{

    /// <summary>
    /// Sercive for handling Models,Weights & Contexts
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Services.IModelService" />
    public class ModelService<T> : IModelService<T> where T : IEquatable<T>, IComparable<T>
    {
        private readonly AsyncLock _modelLock;
        private readonly AsyncLock _contextLock;
        private readonly LLamaStackConfig _configuration;
        private readonly ConcurrentDictionary<string, LLamaStackModel<T>> _modelInstances;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public ModelService(LLamaStackConfig configuration)
        {
            _modelLock = new AsyncLock();
            _contextLock = new AsyncLock();
            _configuration = configuration;
            _modelInstances = new ConcurrentDictionary<string, LLamaStackModel<T>>();
        }


        /// <summary>
        /// Loads a model with the provided configuration.
        /// </summary>
        /// <param name="modelConfig">The model configuration.</param>
        /// <returns></returns>
        public async Task<LLamaStackModel<T>> LoadModel(ModelConfig modelConfig)
        {
            if (_modelInstances.TryGetValue(modelConfig.Name, out var existingModel))
                return existingModel;

            using (await _modelLock.LockAsync())
            {
                if (_modelInstances.TryGetValue(modelConfig.Name, out var model))
                    return model;

                // If in single mode unload any other models
                if (_configuration.ModelLoadType == ModelLoadType.Single
                 || _configuration.ModelLoadType == ModelLoadType.PreloadSingle)
                    await UnloadModels();


                model = new LLamaStackModel<T>(modelConfig);
                _modelInstances.TryAdd(modelConfig.Name, model);
                return model;
            }
        }


        /// <summary>
        /// Loads the models.
        /// </summary>
        public async Task LoadModels()
        {
            if (_configuration.ModelLoadType == ModelLoadType.Single
             || _configuration.ModelLoadType == ModelLoadType.Multiple)
                return;

            foreach (var modelConfig in _configuration.Models)
            {
                await LoadModel(modelConfig);

                //Only preload first model if in SinglePreload mode
                if (_configuration.ModelLoadType == ModelLoadType.PreloadSingle)
                    break;
            }
        }


        /// <summary>
        /// Unloads the model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        public Task UnloadModel(string modelName)
        {
            if (_modelInstances.TryRemove(modelName, out var model))
            {
                model?.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }



        /// <summary>
        /// Unloads all models.
        /// </summary>
        public async Task UnloadModels()
        {
            foreach (var modelName in _modelInstances.Keys)
            {
                await UnloadModel(modelName);
            }
        }


        /// <summary>
        /// Gets a model ny name.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        public Task<LLamaStackModel<T>> GetModel(string modelName)
        {
            _modelInstances.TryGetValue(modelName, out var model);
            return Task.FromResult(model);
        }


        /// <summary>
        /// Gets a context from the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The contextId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Model not found</exception>
        public async Task<LLamaStackContext> GetContext(string modelName, T contextId)
        {
            if (!_modelInstances.TryGetValue(modelName, out var model))
                throw new Exception("Model not found");

            return await model.GetContext(contextId);
        }


        /// <summary>
        /// Creates a context on the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The contextId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Model not found</exception>
        public async Task<LLamaStackContext> CreateContext(string modelName, T contextId)
        {
            if (!_modelInstances.TryGetValue(modelName, out var model))
                throw new Exception("Model not found");

            using (await _contextLock.LockAsync())
            {
                return await model.CreateContext(contextId);
            }
        }


        /// <summary>
        /// Removes a context from the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The contextId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Model not found</exception>
        public async Task<bool> RemoveContext(string modelName, T contextId)
        {
            if (!_modelInstances.TryGetValue(modelName, out var model))
                throw new Exception("Model not found");

            using (await _contextLock.LockAsync())
            {
                return await model.RemoveContext(contextId);
            }
        }


        /// <summary>
        /// Loads, Gets,Creates a Model and a Context
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextId">The contextId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Model option '{modelName}' not found</exception>
        public async Task<(LLamaStackModel<T>, LLamaStackContext)> GetOrCreateModelAndContext(string modelName, T contextId)
        {
            if (_modelInstances.TryGetValue(modelName, out var model))
                return (model, await model.GetContext(contextId) ?? await model.CreateContext(contextId));


            // Get model configuration
            var modelConfig = _configuration.Models.FirstOrDefault(x => x.Name == modelName);
            if (modelConfig is null)
                throw new Exception($"Model option '{modelName}' not found");

            // Load Model
            model = await LoadModel(modelConfig);

            // Get or Create Context
            return (model, await model.GetContext(contextId) ?? await model.CreateContext(contextId));
        }

    }
}
