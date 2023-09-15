using LLama.Abstractions;
using LLamaStack.Core.Common;
using System.Text;

namespace LLamaStack.Core.Inference
{
    public sealed class InstructInferenceHandler<T> : InferenceHandlerBase<T>
    {
        private readonly string _instructionPrefix;
        private readonly string _instructionSuffix;

        private bool _isPromptRun = true;
        private TokenData[] _instructionPrefixTokens;
        private TokenData[] _instructionSuffixTokens;


        public InstructInferenceHandler(LLamaStackModel<T> model, LLamaStackContext context, string instructionPrefix = "\n\n### Instruction:\n\n", string instructionSuffix = "\n\n### Response:\n\n") : base(model, context)
        {
            _instructionPrefix = instructionPrefix;
            _instructionSuffix = instructionSuffix;
            _instructionPrefixTokens = _context.TokenizeTextToArray(_instructionPrefix, true);
            _instructionSuffixTokens = _context.TokenizeTextToArray(_instructionSuffix, false);
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
                _promptTokens = _context.TokenizeTextToList(text, true);
            }
            else
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                _consumedTokensCount = _promptTokens.Count;
                _promptTokens.AddRange(_instructionPrefixTokens);

                var line_inp = _context.TokenizeTextToList(text, false);
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
                        _context.TokenToString(token, last_output_builder);
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

            if (_currentTokens.Count > 0 && _currentTokens.Last()?.Id == _context.TokenEOS)
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
                if (_pastTokensCount + _currentTokens.Count > _context.ContextSize)
                {
                    await HandleRunOutOfContext(inferenceParams.TokensKeep);
                }

                _pastTokensCount = await _context.EvalAsync(_currentTokens, _pastTokensCount);
            }

            _currentTokens.Clear();

            if (_promptTokens.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var tokenData = await _sampleService.SampleAsync(inferenceParams, _lastTokens);

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
                    if (_currentTokens.Count >= _model.ModelParams.BatchSize)
                    {
                        break;
                    }
                }
            }
        }


        public override async Task<InferenceHandlerState> GetStateAsync()
        {
            var state = await base.GetStateAsync();
            state.IsPromptRun = _isPromptRun;
            state.InputPrefixTokens = _instructionPrefixTokens;
            state.InputSuffixTokens = _instructionSuffixTokens;
            state.InferenceType = Type;
            return state;
        }


        public override Task SetStateAsync(InferenceHandlerState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            _isPromptRun = state.IsPromptRun;
            _instructionPrefixTokens = state.InputPrefixTokens;
            _instructionSuffixTokens = state.InputSuffixTokens;
            return base.SetStateAsync(state);
        }
    }
}
