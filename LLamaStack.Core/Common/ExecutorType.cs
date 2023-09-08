
namespace LLamaStack.Core.Common
{
    /// <summary>
    /// LLamaSharp executor type
    /// </summary>
    public enum ExecutorType
    {

        /// <summary>
        /// The interactive executor, for more personal type chatbot interaction
        /// </summary>
        Interactive = 0,


        /// <summary>
        /// The instruct executor, good for instruction/response type interaction
        /// </summary>
        Instruct = 1,


        /// <summary>
        /// The stateless executor, holds no state or context during interaction
        /// </summary>
        Stateless = 2
    }
}
