using LLama;
using LLama.Abstractions;
using LLama.Native;
using LLamaStack.Core.Common;
using System.Text;

namespace LLamaStack.Core.Inference
{
    public class InstructInferenceHandler : InferenceHandlerBase
    {

        private readonly string _instructionPrefix;
        private readonly string _instructionSuffix;

        private bool _isPromptRun = true;
        private TokenData[] _instructionPrefixTokens;
        private TokenData[] _instructionSuffixTokens;


        public InstructInferenceHandler(LLamaContext context, string instructionPrefix = "\n\n### Instruction:\n\n", string instructionSuffix = "\n\n### Response:\n\n") : base(context)
        {
            _instructionPrefix = instructionPrefix;
            _instructionSuffix = instructionSuffix;
            _instructionPrefixTokens = Context.TokenizeTextToArray(_instructionPrefix, true);
            _instructionSuffixTokens = Context.TokenizeTextToArray(_instructionSuffix, false);
        }

        public override InferenceType Type => InferenceType.Instruct;


        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 || _isPromptRun);
        }


        protected override Task PreprocessInputs(string text, InferStateArgs args)
        {
            args.Antiprompts ??= new List<string>();
            args.Antiprompts.Add(_instructionPrefix);
            if (_isPromptRun)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
                _promptTokens = Context.TokenizeTextToList(text, true);
            }
            else
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                _consumedTokensCount = _promptTokens.Count;
                _promptTokens.AddRange(_instructionPrefixTokens);

                var line_inp = Context.TokenizeTextToList(text, false);
                _promptTokens.AddRange(line_inp);

                _promptTokens.AddRange(_instructionSuffixTokens);

                args.RemainedTokens -= line_inp.Count;
            }
            return Task.CompletedTask;
        }


        protected override Task<bool> PostProcess(IInferenceParams inferenceParams, InferStateArgs args)
        {
            if (_promptTokens.Count <= _consumedTokensCount)
            {
                if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                {
                    var last_output_builder = new StringBuilder();
                    foreach (var token in _lastTokens)
                    {
                        Context.NativeHandle.TokenToString(token.Id, Context.Encoding, last_output_builder);
                    }

                    var last_output = last_output_builder.ToString();
                    foreach (var antiprompt in args.Antiprompts)
                    {
                        if (last_output.EndsWith(antiprompt))
                        {
                            args.WaitForInput = true;
                            return Task.FromResult(true);
                        }
                    }
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return Task.FromResult(true);
                }
            }

            if (_currentTokens.Count > 0 && _currentTokens.Last()?.Id == NativeApi.llama_token_eos(Context.NativeHandle))
            {
                args.WaitForInput = true;
            }

            if (args.RemainedTokens <= 0 && inferenceParams.MaxTokens != -1)
            {
                args.RemainedTokens = inferenceParams.MaxTokens;
                args.WaitForInput = true;
            }
            return Task.FromResult(false);
        }


        protected override async Task InferInternal(IInferenceParams inferenceParams, InferStateArgs args)
        {
            if (_currentTokens.Count > 0)
            {
                _isPromptRun = false;
                if (_pastTokensCount + _currentTokens.Count > Context.ContextSize)
                {
                    await HandleRunOutOfContext(inferenceParams.TokensKeep);
                }

                _pastTokensCount = await Context.Eval(_currentTokens, _pastTokensCount);
            }

            _currentTokens.Clear();

            if (_promptTokens.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var tokenDataArray = Context.ApplyPenalty(_lastTokens, inferenceParams);

                var mu = MirostatMu;
                var id = Context.Sample(tokenDataArray, inferenceParams, ref mu);
                MirostatMu = mu;

                var tokenData = tokenDataArray.GetTokenData(Context, id);

                _lastTokens.Enqueue(tokenData);

                _currentTokens.Add(tokenData);

                args.RemainedTokens--;
                args.ReturnValue = true;
            }
            else
            {
                while (_promptTokens.Count > _consumedTokensCount)
                {
                    _currentTokens.Add(_promptTokens[_consumedTokensCount]);
                    _lastTokens.Enqueue(_promptTokens[_consumedTokensCount]);
                    _consumedTokensCount++;
                    if (_currentTokens.Count >= Context.Params.BatchSize)
                    {
                        break;
                    }
                }
            }
        }


        public override async Task<InferenceHandlerState> GetState()
        {
            var state = await base.GetState();
            state.IsPromptRun = _isPromptRun;
            state.InputPrefixTokens = _instructionPrefixTokens;
            state.InputSuffixTokens = _instructionSuffixTokens;
            state.InferenceType = Type;
            return state;
        }


        public override Task SetState(InferenceHandlerState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            _isPromptRun = state.IsPromptRun;
            _instructionPrefixTokens = state.InputPrefixTokens;
            _instructionSuffixTokens = state.InputSuffixTokens;
            return base.SetState(state);
        }
    }
}
