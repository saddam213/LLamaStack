using Microsoft.ML.OnnxRuntime.Tensors;
using NumSharp;
using SixLabors.ImageSharp;
using LLamaStack.StableDiffusion.Config;
using LLamaStack.StableDiffusion.Helpers;

namespace LLamaStack.StableDiffusion.Diffusers
{
    public class EulerAncestralDiffuser : DiffuserBase
    {
        public EulerAncestralDiffuser() : base(new DiffuserConfig()) { }
        public EulerAncestralDiffuser(DiffuserConfig diffuserConfig) : base(diffuserConfig) { }

        protected override void Initialize()
        {
            var alphas = new List<float>();
            var betas = new List<float>();


            if (_diffuserConfig.TrainedBetas != null)
            {
                betas = _diffuserConfig.TrainedBetas.ToList();
            }
            else if (_diffuserConfig.BetaSchedule == DiffuserBetaSchedule.Linear)
            {
                betas = Enumerable.Range(0, _diffuserConfig.TrainTimesteps).Select(i => _diffuserConfig.BetaStart + (_diffuserConfig.BetaEnd - _diffuserConfig.BetaStart) * i / (_diffuserConfig.TrainTimesteps - 1)).ToList();
            }
            else if (_diffuserConfig.BetaSchedule == DiffuserBetaSchedule.ScaledLinear)
            {
                var start = (float)Math.Sqrt(_diffuserConfig.BetaStart);
                var end = (float)Math.Sqrt(_diffuserConfig.BetaEnd);
                betas = np.linspace(start, end, _diffuserConfig.TrainTimesteps).ToArray<float>().Select(x => x * x).ToList();
            }

            alphas = betas.Select(beta => 1 - beta).ToList();

            _alphasCumulativeProducts = alphas.Select((alpha, i) => alphas.Take(i + 1).Aggregate((a, b) => a * b)).ToList();

            // Create sigmas as a list and reverse it
            var sigmas = _alphasCumulativeProducts.Select(alpha_prod => Math.Sqrt((1 - alpha_prod) / alpha_prod)).Reverse().ToList();

            // standard deviation of the initial noise distrubution
            _initNoiseSigma = (float)sigmas.Max();
        }


        public override int[] SetTimesteps(int inferenceStepCount)
        {
            double start = 0;
            double stop = _diffuserConfig.TrainTimesteps - 1;
            double[] timesteps = np.linspace(start, stop, inferenceStepCount).ToArray<double>();

            _timesteps = timesteps.Select(x => (int)x).Reverse().ToList();

            var sigmas = _alphasCumulativeProducts.Select(alpha_prod => Math.Sqrt((1 - alpha_prod) / alpha_prod)).Reverse().ToList();
            var range = np.arange(0, (double)sigmas.Count).ToArray<double>();
            sigmas = Interpolate(timesteps, range, sigmas).ToList();
            _initNoiseSigma = (float)sigmas.Max();
            _sigmas = new DenseTensor<float>(sigmas.Count());
            for (int i = 0; i < sigmas.Count(); i++)
            {
                _sigmas[i] = (float)sigmas[i];
            }
            return _timesteps.ToArray();

        }

        public override DenseTensor<float> Step(Tensor<float> modelOutput, int timestep, Tensor<float> sample, int order = 4)
        {
            if (!_isScaleInputCalled)
                throw new Exception("The `scale_model_input` function should be called before `step` to ensure correct denoising. ");

            int stepIndex = _timesteps.IndexOf(timestep);
            var sigma = _sigmas[stepIndex];

            // 1. compute predicted original sample (x_0) from sigma-scaled predicted noise
            var predOriginalSample = TensorHelper.SubtractTensors(sample, TensorHelper.MultipleTensorByFloat(modelOutput, sigma));


            float sigmaFrom = _sigmas[stepIndex];
            float sigmaTo = _sigmas[stepIndex + 1];

            var sigmaFromLessSigmaTo = MathF.Pow(sigmaFrom, 2) - MathF.Pow(sigmaTo, 2);
            var sigmaUpResult = MathF.Pow(sigmaTo, 2) * sigmaFromLessSigmaTo / MathF.Pow(sigmaFrom, 2);
            var sigmaUp = sigmaUpResult < 0 ? -MathF.Pow(MathF.Abs(sigmaUpResult), 0.5f) : MathF.Pow(sigmaUpResult, 0.5f);

            var sigmaDownResult = MathF.Pow(sigmaTo, 2) - MathF.Pow(sigmaUp, 2);
            var sigmaDown = sigmaDownResult < 0 ? -MathF.Pow(MathF.Abs(sigmaDownResult), 0.5f) : MathF.Pow(sigmaDownResult, 0.5f);

            // 2. Convert to an ODE derivative
            var sampleMinusPredOriginalSample = TensorHelper.SubtractTensors(sample, predOriginalSample);
            DenseTensor<float> derivative = TensorHelper.DivideTensorByFloat(sampleMinusPredOriginalSample, sigma, predOriginalSample.Dimensions);

            float dt = sigmaDown - sigma;

            DenseTensor<float> prevSample = TensorHelper.AddTensors(sample, TensorHelper.MultipleTensorByFloat(derivative, dt));

            var noise = TensorHelper.GetRandomTensor(prevSample.Dimensions);

            var noiseSigmaUpProduct = TensorHelper.MultipleTensorByFloat(noise, sigmaUp);
            prevSample = TensorHelper.AddTensors(prevSample, noiseSigmaUpProduct);

            return prevSample;
        }

    }
}
