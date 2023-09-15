namespace LLamaStack.Core.Inference
{
    public class TokenOutputStreamTransform : ITokenStreamTransform
    {
        private readonly HashSet<string> _keywords;
        private readonly int _maxKeywordLength;

        public TokenOutputStreamTransform(IEnumerable<string> keywords, int redundancyLength = 3)
        {
            _keywords = new(keywords);
            _maxKeywordLength = _keywords.Max(x => x.Length) + redundancyLength;
            _maxKeywordLength = _keywords.Select(x => x.Length).Max() + redundancyLength;
        }

        public async IAsyncEnumerable<TokenData> TransformAsync(IAsyncEnumerable<TokenData> tokens)
        {
            var window = new Queue<TokenData>();
            await foreach (var s in tokens)
            {
                window.Enqueue(s);
                var current = JoinTokenContent(window);
                if (_keywords.Any(current.Contains))
                {
                    var matchedKeyword = _keywords.First(current.Contains);
                    while (matchedKeyword != current)
                    {
                        if (window.Count == 0)
                            break;

                        yield return window.Dequeue();
                        current = JoinTokenContent(window);
                    }

                    int total = window.Count;
                    for (int i = 0; i < total; i++)
                    {
                        window.Dequeue();
                    }
                }
                if (current.Length >= _maxKeywordLength)
                {
                    if (window.Count > 0)
                        yield return window.Dequeue();
                }
            }
            int totalCount = window.Count;
            for (int i = 0; i < totalCount; i++)
            {
                yield return window.Dequeue();
            }
        }

        private static string JoinTokenContent(Queue<TokenData> window)
        {
            return string.Join("", window.Select(x => x.Content)).Trim();
        }
    }
}

