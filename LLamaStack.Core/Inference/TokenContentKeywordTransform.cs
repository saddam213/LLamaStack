
namespace LLamaStack.Core.Inference
{
    /// <summary>
    /// Searches the TokenData stream for kewords and removes the tokens from the stream
    /// </summary>
    public class TokenContentKeywordTransform : ITokenStreamTransform
    {
        private readonly int _searchWindowLength;
        private readonly HashSet<string> _keywords;


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenContentKeywordTransform"/> class.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="redundancyLength">Length of the redundancy.</param>
        public TokenContentKeywordTransform(IEnumerable<string> keywords)
        {
            _keywords = new(keywords);
            _searchWindowLength = _keywords.Max(x => x.Length);
        }


        /// <summary>
        /// Transforms the asynchronous.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        public async IAsyncEnumerable<TokenData> TransformAsync(IAsyncEnumerable<TokenData> tokens)
        {
            var bucket = new Queue<TokenData>();
            await foreach (var s in tokens)
            {
                bucket.Enqueue(s);
                var currentWindow = JoinTokenContent(bucket);
                if (_keywords.Any(currentWindow.Contains))
                {
                    // Keyword found, check which one
                    var matchedKeyword = _keywords.First(currentWindow.Contains);

                    // Keep dequeuing tokens until just the keyword remains
                    while (matchedKeyword != currentWindow)
                    {
                        if (bucket.Count == 0)
                            break;

                        yield return bucket.Dequeue();
                        currentWindow = JoinTokenContent(bucket);
                    }

                    // Only keyword remains, dequeue into the ether
                    int total = bucket.Count;
                    for (int i = 0; i < total; i++)
                        bucket.Dequeue();
                }

                // Window is longer than the max keyword length, start dequeuing vaild tokens
                if (currentWindow.Length >= _searchWindowLength)
                {
                    if (bucket.Count > 0)
                        yield return bucket.Dequeue();
                }
            }

            // Finished the loop, empty the queue
            int totalCount = bucket.Count;
            for (int i = 0; i < totalCount; i++)
                yield return bucket.Dequeue();
        }


        /// <summary>
        /// Joins the content of the token in the bucket.
        /// </summary>
        /// <param name="bucket">The bucket.</param>
        /// <returns></returns>
        private static string JoinTokenContent(Queue<TokenData> bucket)
        {
            return string.Join("", bucket.Select(x => x.Content)).TrimStart();
        }
    }
}

