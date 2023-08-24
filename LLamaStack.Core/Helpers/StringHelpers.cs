namespace LLamaStack.Core.Helpers
{
    public static class StringHelpers
    {
        public static List<string> CommaSeperatedToList(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim())
                 .ToList();
        }
    }
}
