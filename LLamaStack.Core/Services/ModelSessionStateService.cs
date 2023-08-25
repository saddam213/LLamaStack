using LLama.Abstractions;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Helpers;
using LLamaStack.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using static LLama.InstructExecutor;
using static LLama.InteractiveExecutor;

namespace LLamaStack.Core.Services
{

    /// <summary>
    /// Service for handling ModelSessionStates
    /// </summary>
    /// <typeparam name="T">Type used for the session identifier</typeparam>
    /// <seealso cref="LLamaStack.Core.Services.IModelSessionStateService&lt;T&gt;" />
    public class ModelSessionStateService<T> : IModelSessionStateService<T>
        where T : IEquatable<T>, IComparable<T>
    {
        private const string FilenameContextState = "ContextState.bin";
        private const string FilenameSessionState = "SessionState.json";
        private const string FilenameExecutorState = "ExecutorState.json";

        private readonly AsyncLock _asyncLock;
        private readonly LLamaStackConfig _configuration;
        private readonly ILogger<ModelSessionStateService<T>> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly JsonSerializerOptions _jsonDeserializerOptions;
        private readonly ConcurrentDictionary<T, ModelSessionState<T>> _savedSessions;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSessionStateService{T}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public ModelSessionStateService(ILogger<ModelSessionStateService<T>> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _asyncLock = new AsyncLock();
            _savedSessions = new ConcurrentDictionary<T, ModelSessionState<T>>();
            _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            _jsonDeserializerOptions = new JsonSerializerOptions { Converters = { new JsonInterfaceConverter<SessionConfig, ISessionConfig>(), new JsonInterfaceConverter<InferenceConfig, IInferenceParams>() } };
        }


        /// <summary>
        /// Gets ModleSessionState by id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> GetAsync(T sessionId)
        {
            var sessions = await GetAllAsync();
            return sessions.FirstOrDefault(x => x.Id.ToString() == sessionId.ToString());
        }


        /// <summary>
        /// Gets all ModelSessionStates.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ModelSessionState<T>>> GetAllAsync()
        {
            await LoadAllFromDirectory();
            return _savedSessions.Values;
        }


        /// <summary>
        /// Loads a saved ModelSessionState
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> LoadAsync(T sessionId, CancellationToken cancellationToken = default)
        {
            // Load from cache
            if (_savedSessions.TryGetValue(sessionId, out var exsitingSession))
                return exsitingSession;

            // Load from directory
            var sessionStateDirectory = GetSessionPath(sessionId);
            var modelSessionState = await LoadFromDirectory(sessionStateDirectory);

            // Add to cache
            AddOrUpdateSavedModelSession(modelSessionState);
            return modelSessionState;
        }


        /// <summary>
        /// Saves the ModelSessionState
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="modelSession">The model session.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> SaveAsync(T sessionId, ModelSession<T> modelSession, CancellationToken cancellationToken = default)
        {
            //Save session state
            var sessionStateDirectory = GetSessionPath(sessionId);
            var modelSessionState = await SaveToDirectory(modelSession.GetState(), sessionStateDirectory);

            // Add to cache
            AddOrUpdateSavedModelSession(modelSessionState);
            return modelSessionState;
        }


        /// <summary>
        /// Removes a ModelSessionState
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(T sessionId)
        {
            // Delete Directory
            var sessionStateDirectory = GetSessionPath(sessionId);
            if (await DeleteDirectory(sessionStateDirectory))
            {
                // Remove Cache
                _savedSessions.TryRemove(sessionId, out _);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Deletes the ModelSessionState directory.
        /// </summary>
        /// <param name="sessionStateDirectory">The session state directory.</param>
        /// <returns></returns>
        private Task<bool> DeleteDirectory(string sessionStateDirectory)
        {
            if (!Directory.Exists(sessionStateDirectory))
                return Task.FromResult(false);

            Directory.Delete(sessionStateDirectory, true);
            return Task.FromResult(true);
        }


        /// <summary>
        /// Loads a ModelSessionState from a directory.
        /// </summary>
        /// <param name="sessionStateDirectory">The session state directory.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session file is corrupt or out of date
        /// or
        /// Unable to load session, model '{modelSessionState.SessionConfig.Model}' is no longer available, Session: {modelSessionState.Id}
        /// or
        /// Unable to load session, context sizes are not equal, Session: {modelSessionState.Id}
        /// </exception>
        private async Task<ModelSessionState<T>> LoadFromDirectory(string sessionStateDirectory)
        {
            var contextStateFile = Path.Combine(sessionStateDirectory, FilenameContextState);
            var sessionStateFile = Path.Combine(sessionStateDirectory, FilenameSessionState);
            var executorStateFile = Path.Combine(sessionStateDirectory, FilenameExecutorState);
            if (!File.Exists(contextStateFile))
                throw new Exception($"{FilenameContextState} not found");
            if (!File.Exists(sessionStateFile))
                throw new Exception($"{FilenameSessionState} not found");
            if (!File.Exists(sessionStateFile))
                throw new Exception($"{FilenameExecutorState} not found");

            // Session state file
            using var sessionStateStream = File.OpenRead(sessionStateFile);
            var modelSessionState = await JsonSerializer.DeserializeAsync<ModelSessionState<T>>(sessionStateStream, _jsonDeserializerOptions);
            if (modelSessionState is null)
                throw new Exception($"Session file is corrupt or out of date");

            // Executor state file
            using var executorStateStream = File.OpenRead(executorStateFile);
            if (modelSessionState.SessionConfig.ExecutorType == LLamaExecutorType.Instruct)
                modelSessionState.ExecutorConfig = await JsonSerializer.DeserializeAsync<InstructExecutorState>(executorStateStream, _jsonDeserializerOptions);
            else if (modelSessionState.SessionConfig.ExecutorType == LLamaExecutorType.Interactive)
                modelSessionState.ExecutorConfig = await JsonSerializer.DeserializeAsync<InteractiveExecutorState>(executorStateStream, _jsonDeserializerOptions);

            // Get model config for this session
            var modelConfig = _configuration.Models.FirstOrDefault(x => x.Name == modelSessionState.SessionConfig.Model);
            if (modelConfig is null)
                throw new Exception($"Unable to load session, model '{modelSessionState.SessionConfig.Model}' is no longer available, Session: {modelSessionState.Id}");

            //Context sizes must be the same
            if (modelSessionState.ContextSize != modelConfig.ContextSize)
                throw new Exception($"Unable to load session, context sizes are not equal, Session: {modelSessionState.Id}");

            modelSessionState.ContextFile = contextStateFile;
            return modelSessionState;
        }


        /// <summary>
        /// Saves a ModelSessionState to a directory.
        /// </summary>
        /// <param name="modelSessionState">State of the model session.</param>
        /// <param name="sessionStateDirectory">The session state directory.</param>
        /// <returns></returns>
        private async Task<ModelSessionState<T>> SaveToDirectory(ModelSessionState<T> modelSessionState, string sessionStateDirectory)
        {
            var contextStateFile = Path.Combine(sessionStateDirectory, FilenameContextState);
            var sessionStateFile = Path.Combine(sessionStateDirectory, FilenameSessionState);
            var executorStateFile = Path.Combine(sessionStateDirectory, FilenameExecutorState);
            Directory.CreateDirectory(sessionStateDirectory);

            // Save Session state
            using var modelSessionStream = File.Open(sessionStateFile, FileMode.Create);
            await JsonSerializer.SerializeAsync(modelSessionStream, modelSessionState, _jsonSerializerOptions);

            // Save Executor state
            using var executorStateStream = File.Open(executorStateFile, FileMode.Create);
            if (modelSessionState.SessionConfig.ExecutorType == LLamaExecutorType.Instruct)
                await JsonSerializer.SerializeAsync(executorStateStream, modelSessionState.ExecutorConfig as InstructExecutorState, _jsonSerializerOptions);
            else if (modelSessionState.SessionConfig.ExecutorType == LLamaExecutorType.Interactive)
                await JsonSerializer.SerializeAsync(executorStateStream, modelSessionState.ExecutorConfig as InteractiveExecutorState, _jsonSerializerOptions);

            modelSessionState.ContextFile = contextStateFile;
            return modelSessionState;
        }


        /// <summary>
        /// Loads all ModelSessionState from the ModelStatePath directory.
        /// </summary>
        private async Task LoadAllFromDirectory()
        {
            using (await _asyncLock.LockAsync())
            {
                if (_savedSessions.Count > 0 || !Directory.Exists(_configuration.ModelStatePath))
                    return;

                foreach (var sessionStateDirectory in Directory.EnumerateDirectories(_configuration.ModelStatePath, "*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        AddOrUpdateSavedModelSession(await LoadFromDirectory(sessionStateDirectory));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[LoadSavedModelSessions] - {ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// Adds the or update saved model session cache.
        /// </summary>
        /// <param name="modelSessionState">State of the model session.</param>
        private void AddOrUpdateSavedModelSession(ModelSessionState<T> modelSessionState)
        {
            _savedSessions.TryRemove(modelSessionState.Id, out _);
            _savedSessions.TryAdd(modelSessionState.Id, modelSessionState);
        }


        /// <summary>
        /// Gets the session path.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        private string GetSessionPath(T sessionId)
        {
            return Path.Combine(_configuration.ModelStatePath, sessionId.ToString());
        }
    }
}
