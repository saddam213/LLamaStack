using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    public class LLamaStackConfig : IConfigSection
    {
        public ModelLoadType ModelLoadType { get; set; }
        public string ModelStatePath { get; set; }
        public List<ModelConfig> Models { get; set; }

        public void Initialize()
        {
            if (string.IsNullOrEmpty(ModelStatePath))
                ModelStatePath = Path.Combine(Environment.CurrentDirectory, "States");
        }
    }
}
