using LLamaStack.Common.Config;
using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    /// <summary>
    /// LLamaStack appsettings.json config element
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Config.IConfigSection" />
    public class LLamaStackConfig : IConfigSection
    {
        /// <summary>
        /// Gets or sets the ModelLoad type
        /// </summary>
        public ModelLoadType ModelLoadType { get; set; }

        /// <summary>
        /// Gets or sets the model state path.
        /// </summary>
        public string ModelStatePath { get; set; }


        /// <summary>
        /// Gets or sets the models.
        /// </summary>
        public List<ModelConfig> Models { get; set; }

        /// <summary>
        /// Perform any initialization, called directly after deserialization
        /// </summary>
        public void Initialize()
        {
            if (string.IsNullOrEmpty(ModelStatePath))
                ModelStatePath = Path.Combine(Environment.CurrentDirectory, "States");
        }
    }
}
