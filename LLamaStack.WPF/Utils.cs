using LLamaStack.Core.Common;
using System.Collections.Generic;

namespace LLamaStack.WPF
{
    public static class Utils
    {

        /// <summary>
        /// Gets the default prompt configuration.
        /// </summary>
        /// <value>
        /// The default prompt configuration.
        /// </value>
        public static Dictionary<ExecutorType, SessionConfiguration> DefaultPromptConfiguration { get; } = new Dictionary<ExecutorType, SessionConfiguration>
        {
            {
                ExecutorType.Instruct, new SessionConfiguration
                {
                    Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request."
                }
            },
            {
                ExecutorType.Interactive, new SessionConfiguration
                {
                    Prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:",
                    AntiPrompt = "User:"
                }
            },
            {
                 ExecutorType.Stateless, new SessionConfiguration()
            }
        };
    }
}
