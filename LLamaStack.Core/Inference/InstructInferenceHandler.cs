﻿using LLama.Abstractions;
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


        /// <summary>
        /// Initializes a new instance of the <see cref="InstructInferenceHandler{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="instructionPrefix">The instruction prefix.</param>
        /// <param name="instructionSuffix">The instruction suffix.</param>
        public InstructInferenceHandler(LLamaStackModel<T> model, LLamaStackContext context, string instructionPrefix = "\n\n### Instruction:\n\n", string instructionSuffix = "\n\n### Response:\n\n") : base(model, context)
        {
            _instructionPrefix = instructionPrefix;
            _instructionSuffix = instructionSuffix;
            _instructionPrefixTokens = _context.TokenizeTextToArray(_instructionPrefix, true);
            _instructionSuffixTokens = _context.TokenizeTextToArray(_instructionSuffix, false);
        }


        /// <summary>
        /// Gets the InferenceType.
        /// </summary>
        public override InferenceType Type => InferenceType.Instruct;


        /// <summary>
        /// Decide whether to continue the loop.
        /// </summary>
        /// <param name="args">The state args</param>
        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 || _isPromptRun);
        }


        /// <summary>
        /// Preprocess the inputs before the inference.
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="args">The state args</param>
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


        /// <summary>
        /// Do some post processing after the inference.
        /// </summary>
        /// <param name="inferenceParams">The inferenceParams</param>
        /// <param name="args">The state args</param>
        protected override Task<bool> PostProcess(IInferenceParams inferenceParams, InferStateArgs args)
        {
            if (_promptTokens.Count <= _consumedTokensCount)
            {
                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return Task.FromResult(true);
                }
            }

            if (_currentTokens.Count > 0 && _currentTokens.Last()?.Id == _model.TokenEOS)
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


        /// <summary>
        /// The core inference logic.
        /// </summary>
        /// <param name="inferenceParams">The inferenceParams</param>
        /// <param name="args">The state args</param>
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
                    if (_currentTokens.Count >= _model.LLamaParams.BatchSize)
                    {
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Gets the state.
        /// </summary>
        public override async Task<InferenceHandlerState> GetStateAsync()
        {
            var state = await base.GetStateAsync();
            state.IsPromptRun = _isPromptRun;
            state.InputPrefixTokens = _instructionPrefixTokens;
            state.InputSuffixTokens = _instructionSuffixTokens;
            state.InferenceType = Type;
            return state;
        }


        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
