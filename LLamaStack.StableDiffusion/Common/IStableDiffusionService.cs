using LLamaStack.StableDiffusion.Config;

namespace LLamaStack.StableDiffusion.Common
{
    public interface IStableDiffusionService : IDisposable
    {
        Task<Image<Rgba32>> TextToImage(string prompt);
        Task<Image<Rgba32>> TextToImage(string prompt, DiffuserConfig diffuserConfig);

        Task<bool> TextToImageFile(string prompt, string filename);
        Task<bool> TextToImageFile(string prompt, string filename, DiffuserConfig diffuserConfig);
    }
}