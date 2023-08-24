using LLama.Abstractions;
using LLamaStack.Core.Config;
using System.Text.Json.Serialization;
using static LLama.StatefulExecutorBase;

namespace LLamaStack.Core.Models
{
    public class ModelSessionState<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public int ContextSize { get; set; }
        public DateTime Created { get; set; }
        public ISessionConfig SessionConfig { get; set; }
        public IInferenceParams InferenceConfig { get; set; }
        public List<SessionHistoryModel> SessionHistory { get; set; } = new List<SessionHistoryModel>();


        [JsonIgnore]
        public string ContextFile { get; set; }

        [JsonIgnore]
        public ExecutorBaseState ExecutorConfig { get; set; }
    }

}
