
namespace LLamaStack.Core.Common
{
    /// <summary>
    /// The type of token sampling algo to use
    /// </summary>
    public enum SamplerType
    {
        /// <summary>
        /// The default implemetation
        /// </summary>
        Default = 0,

        /// <summary>
        /// Original mirostat algorithm
        /// </summary>
        Mirostatv1 = 1,

        /// <summary>
        /// Mirostat 2.0 algorithm
        /// </summary>
        Mirostatv2 = 2
    }
}
