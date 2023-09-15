
namespace LLamaStack.Core.Common
{
    /// <summary>
    /// Inference type
    /// </summary>
    public enum InferenceType
    {

        /// <summary>
        /// Interactive, for more personal type chatbot interaction
        /// </summary>
        Interactive = 0,


        /// <summary>
        /// Instruct, good for instruction/response type interaction
        /// </summary>
        Instruct = 1,


        /// <summary>
        /// Stateless, holds no state or context during interaction
        /// </summary>
        Stateless = 2
    }
}
