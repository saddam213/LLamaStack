using LLamaStack.Core.Config;
using LLamaStack.Core.Helpers;

namespace LLamaStack.Core.Extensions
{
    public static class Extensions
    {

        /// <summary>
        /// Combines the AntiPrompts list and AntiPrompt csv 
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <returns>Combined AntiPrompts with duplicates removed</returns>
        public static List<string> GetAntiPrompts(this ISessionConfig sessionConfig)
        {
            return CombineCSV(sessionConfig.AntiPrompts, sessionConfig.AntiPrompt);
        }

        /// <summary>
        /// Combines the OutputFilters list and OutputFilter csv 
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <returns>Combined OutputFilters with duplicates removed</returns>
        public static List<string> GetOutputFilters(this ISessionConfig sessionConfig)
        {
            return CombineCSV(sessionConfig.OutputFilters, sessionConfig.OutputFilter);
        }


        /// <summary>
        /// Combines a string list and a csv and removes duplicates
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="csv">The CSV.</param>
        /// <returns>Combined list with duplicates removed</returns>
        private static List<string> CombineCSV(List<string> list, string csv)
        {
            var results = list?.Count == 0
                ? StringHelpers.CommaSeperatedToList(csv)
                : StringHelpers.CommaSeperatedToList(csv).Concat(list);
            return results
                .Distinct()
                .ToList();
        }
    }
}
