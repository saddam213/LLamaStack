namespace LLamaStack.Core.Helpers
{
    public static class StringHelpers
    {
        public static List<string> CommaSeperatedToList(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new List<string>();

            return value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim())
                 .ToList();
        }

    }
}
