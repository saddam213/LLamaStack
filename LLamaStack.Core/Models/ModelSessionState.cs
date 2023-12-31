﻿using LLamaStack.Core.Config;
using LLamaStack.Core.Inference;
using System.Text.Json.Serialization;

namespace LLamaStack.Core.Models
{
    public class ModelSessionState<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public int ContextSize { get; set; }
        public DateTime Created { get; set; }
        public ISessionConfig SessionConfig { get; set; }
        public IInferenceConfig InferenceConfig { get; set; }
        public List<SessionHistoryModel> SessionHistory { get; set; } = new List<SessionHistoryModel>();


        [JsonIgnore]
        public string ContextFile { get; set; }

        [JsonIgnore]
        public InferenceHandlerState InferenceState { get; set; }
    }

}
