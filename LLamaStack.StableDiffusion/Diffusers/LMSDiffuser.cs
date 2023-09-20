using MathNet.Numerics;
using Microsoft.ML.OnnxRuntime.Tensors;
using NumSharp;
using LLamaStack.StableDiffusion.Config;
using LLamaStack.StableDiffusion.Helpers;

namespace LLamaStack.StableDiffusion.Diffusers
{
    public class LMSDiffuser : DiffuserBase
    {
        private readonly List<Tensor<float>> _derivatives;

        public LMSDiffuser() : this(new DiffuserConfig()) { }

        public LMSDiffuser(DiffuserConfig configuration) : base(configuration)
        {
            _derivatives = new List<Tensor<float>>();
        }

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






        //python line 135 of scheduling_lms_discrete.py
        public double GetLmsCoefficient(int order, int t, int currentOrder)
        {
            // Compute a linear multistep coefficient.

            double LmsDerivative(double tau)
            {
                double prod = 1.0;
                for (int k = 0; k < order; k++)
                {
                    if (currentOrder == k)
                    {
                        continue;
                    }
                    prod *= (tau - _sigmas[t - k]) / (_sigmas[t - currentOrder] - _sigmas[t - k]);
                }
                return prod;
            }

            double integratedCoeff = Integrate.OnClosedInterval(LmsDerivative, _sigmas[t], _sigmas[t + 1], 1e-4);

            return integratedCoeff;
        }

        // Line 157 of scheduling_lms_discrete.py from HuggingFace diffusers
        public override int[] SetTimesteps(int num_inference_steps)
        {
            double start = 0;
            double stop = _diffuserConfig.TrainTimesteps - 1;
            double[] timesteps = np.linspace(start, stop, num_inference_steps).ToArray<double>();

            _timesteps = timesteps.Select(x => (int)x).Reverse().ToList();

            var sigmas = _alphasCumulativeProducts.Select(alpha_prod => Math.Sqrt((1 - alpha_prod) / alpha_prod)).Reverse().ToList();
            var range = np.arange(0, (double)sigmas.Count).ToArray<double>();
            sigmas = Interpolate(timesteps, range, sigmas).ToList();
            _sigmas = new DenseTensor<float>(sigmas.Count());
            for (int i = 0; i < sigmas.Count(); i++)
            {
                _sigmas[i] = (float)sigmas[i];
            }
            return _timesteps.ToArray();

        }

        public override DenseTensor<float> Step(Tensor<float> modelOutput, int timestep, Tensor<float> sample, int order = 4)
        {
            int stepIndex = _timesteps.IndexOf(timestep);
            var sigma = _sigmas[stepIndex];

            // Create array of type float length modelOutput.length
            float[] predOriginalSampleArray = new float[modelOutput.Length];
            var modelOutPutArray = modelOutput.ToArray();
            var sampleArray = sample.ToArray();

            for (int i = 0; i < modelOutPutArray.Length; i++)
            {
                predOriginalSampleArray[i] = sampleArray[i] - sigma * modelOutPutArray[i];
            }

            // 1. compute predicted original sample (x_0) from sigma-scaled predicted noise
            var predOriginalSample = TensorHelper.CreateTensor(predOriginalSampleArray, modelOutput.Dimensions);


            // 2. Convert to an ODE derivative
            var derivativeItems = new DenseTensor<float>(sample.Dimensions);

            var derivativeItemsArray = new float[derivativeItems.Length];

            for (int i = 0; i < modelOutPutArray.Length; i++)
            {
                //predOriginalSample = (sample - predOriginalSample) / sigma;
                derivativeItemsArray[i] = (sampleArray[i] - predOriginalSampleArray[i]) / sigma;
            }
            derivativeItems = TensorHelper.CreateTensor(derivativeItemsArray, derivativeItems.Dimensions);

            _derivatives.Add(derivativeItems);

            if (_derivatives.Count > order)
            {
                // remove first element
                _derivatives.RemoveAt(0);
            }

            // 3. compute linear multistep coefficients
            order = Math.Min(stepIndex + 1, order);
            var lmsCoeffs = Enumerable.Range(0, order).Select(currOrder => GetLmsCoefficient(order, stepIndex, currOrder)).ToArray();

            // 4. compute previous sample based on the derivative path
            // Reverse list of tensors this.derivatives
            var revDerivatives = Enumerable.Reverse(_derivatives).ToList();

            // Create list of tuples from the lmsCoeffs and reversed derivatives
            var lmsCoeffsAndDerivatives = lmsCoeffs.Zip(revDerivatives, (lmsCoeff, derivative) => (lmsCoeff, derivative));

            // Create tensor for product of lmscoeffs and derivatives
            var lmsDerProduct = new Tensor<float>[_derivatives.Count];

            for (int m = 0; m < lmsCoeffsAndDerivatives.Count(); m++)
            {
                var item = lmsCoeffsAndDerivatives.ElementAt(m);
                // Multiply to coeff by each derivatives to create the new tensors
                lmsDerProduct[m] = TensorHelper.MultipleTensorByFloat(item.derivative, (float)item.lmsCoeff, item.derivative.Dimensions);
            }
            // Sum the tensors
            var sumTensor = TensorHelper.SumTensors(lmsDerProduct, new[] { 1, 4, 64, 64 });

            // Add the sumed tensor to the sample
            var prevSample = TensorHelper.AddTensors(sample, sumTensor, sample.Dimensions);

            return prevSample;

        }
    }
}
