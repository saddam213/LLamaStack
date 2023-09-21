using Microsoft.ML.OnnxRuntime.Tensors;
using LLamaStack.StableDiffusion.Config;

namespace LLamaStack.StableDiffusion.Common
{
    public interface IInferenceService : IDisposable
    {
        int[] TokenizeText(string text);
        DenseTensor<float> PreprocessText(string prompt, string negativePrompt);
        Tensor<float> RunInference(string prompt, DiffuserConfig diffuserConfig);
        Tensor<float> RunInference(string prompt, string negativePrompt, DiffuserConfig diffuserConfig);
    }
}