using Microsoft.ML.OnnxRuntime.Tensors;

namespace LLamaStack.StableDiffusion.Common
{
    public interface IImageService
    {
        Image<Rgba32> TensorToImage(Tensor<float> imageTensor, int width, int height);
    }
}