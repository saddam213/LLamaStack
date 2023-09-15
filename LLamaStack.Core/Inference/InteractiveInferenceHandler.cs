using LLama.Abstractions;
using LLamaStack.Core.Common;
using System.Text;

namespace LLamaStack.Core.Inference
{
    public sealed class InteractiveInferenceHandler<T> : InferenceHandlerBase<T>
    {
        private bool _isPromptRun = true;
        private readonly TokenData _tokenNewline;

        public InteractiveInferenceHandler(LLamaStackModel<T> model, LLamaStackContext context) : base(model, context)
        {
            _tokenNewline = new TokenData(_context.TokenNL);
        }

        public override InferenceType Type => InferenceType.Instruct;


        /// <summary>
        /// Define whether to continue the loop to generate responses.
        /// </summary>
        /// <returns></returns>
        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 && !args.WaitForInput || _isPromptRun);
        }


        protected override Task PreprocessInputs(string text, InferStateArgs args)
        {
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
                var line_inp = _context.TokenizeTextToList(text, false);
                _promptTokens.AddRange(line_inp);
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
                            break;
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
                return Task.FromResult(true);
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


                if (tokenData.Id == _context.TokenEOS)
                {
                    tokenData = _tokenNewline;
                    if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                    {
                        var first_antiprompt = _context.TokenizeTextToList(args.Antiprompts[0], false);
                        _promptTokens.AddRange(first_antiprompt);
                    }
                }

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
            state.InferenceType = Type;
            return state;
        }

        public override Task SetStateAsync(InferenceHandlerState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            _isPromptRun = state.IsPromptRun;
            return base.SetStateAsync(state);
        }
    }
}
