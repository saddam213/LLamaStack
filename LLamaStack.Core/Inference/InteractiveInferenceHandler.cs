using LLama;
using LLama.Abstractions;
using LLama.Native;
using LLamaStack.Core.Common;
using System.Text;

namespace LLamaStack.Core.Inference
{
    internal class InteractiveInferenceHandler : InferenceHandlerBase
    {
        private bool _is_prompt_run = true;
        private readonly TokenData _llama_token_newline;


        public InteractiveInferenceHandler(LLamaContext context) : base(context)
        {
            _llama_token_newline = new TokenData(NativeApi.llama_token_nl(Context.NativeHandle));
        }

        public override InferenceType Type => InferenceType.Instruct;


        /// <summary>
        /// Define whether to continue the loop to generate responses.
        /// </summary>
        /// <returns></returns>
        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 && !args.WaitForInput || _is_prompt_run);
        }


        protected override Task PreprocessInputs(string text, InferStateArgs args)
        {
            if (_is_prompt_run)
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
                var line_inp = Context.TokenizeTextToList(text, false);
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
                        Context.NativeHandle.TokenToString(token.Id, Context.Encoding, last_output_builder);
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

            if (_currentTokens.Count > 0 && _currentTokens.Last()?.Id == NativeApi.llama_token_eos(Context.NativeHandle))
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
                _is_prompt_run = false;
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


                if (id == NativeApi.llama_token_eos(Context.NativeHandle))
                {
                    tokenData = _llama_token_newline;
                    if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                    {
                        var first_antiprompt = Context.TokenizeTextToList(args.Antiprompts[0], false);
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
            state.IsPromptRun = _is_prompt_run;
            state.InferenceType = Type;
            return state;
        }

        public override Task SetState(InferenceHandlerState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            _is_prompt_run = state.IsPromptRun;
            return base.SetState(state);
        }
    }
}
