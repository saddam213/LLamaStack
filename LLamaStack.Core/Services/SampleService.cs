using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core.Inference;

namespace LLamaStack.Core.Services
{
    public class SampleService : ISampleService
    {
        private readonly LLamaStackContext _context;

        public SampleService(LLamaStackContext context)
        {
            _context = context;
        }

        public float? MirostatMu { get; set; }

        public Task<TokenData> SampleAsync(IInferenceParams inferenceParams, IEnumerable<TokenData> lastTokens)
        {
            var tokenDataArray = _context.ApplyPenalty(lastTokens, inferenceParams);
            var mu = MirostatMu;
            var id = _context.Sample(tokenDataArray, inferenceParams, ref mu);
            MirostatMu = mu;
            return Task.FromResult(_context.GetTokenData(tokenDataArray, id));
        }
    }
}
