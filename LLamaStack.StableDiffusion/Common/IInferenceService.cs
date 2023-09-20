using Microsoft.ML.OnnxRuntime.Tensors;
using LLamaStack.StableDiffusion.Config;

namespace LLamaStack.StableDiffusion.Common
{
    public interface IInferenceService : IDisposable
    {
        DenseTensor<float> PreprocessText(string prompt);
        Tensor<float> RunInference(string prompt, DiffuserConfig diffuserConfig);
        int[] TokenizeText(string text);
    }
}