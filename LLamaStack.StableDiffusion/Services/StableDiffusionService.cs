using LLamaStack.StableDiffusion.Common;
using LLamaStack.StableDiffusion.Config;

namespace LLamaStack.StableDiffusion.Services
{
    public class StableDiffusionService : IStableDiffusionService
    {
        private readonly IImageService _imageService;
        private readonly IInferenceService _inferenceService;
        private readonly StableDiffusionConfig _configuration;

        public StableDiffusionService(StableDiffusionConfig configuration)
        {
            _configuration = configuration;
            _imageService = new ImageService();
            _inferenceService = new InferenceService(_configuration);
        }

        public Task<Image<Rgba32>> TextToImage(string prompt)
        {
            return TextToImageInternal(prompt, null, new DiffuserConfig());
        }

        public Task<Image<Rgba32>> TextToImage(string prompt, string negativePrompt)
        {
            return TextToImageInternal(prompt, negativePrompt, new DiffuserConfig());
        }

        public Task<Image<Rgba32>> TextToImage(string prompt, DiffuserConfig diffuserConfig)
        {
            return TextToImageInternal(prompt, null, diffuserConfig);
        }

        public Task<Image<Rgba32>> TextToImage(string prompt, string negativePrompt, DiffuserConfig diffuserConfig)
        {
            return TextToImageInternal(prompt, negativePrompt, diffuserConfig);
        }


        public Task<bool> TextToImageFile(string prompt, string filename)
        {
            return TextToImageFileInternal(prompt, null, filename, new DiffuserConfig());
        }

        public Task<bool> TextToImageFile(string prompt, string negativePrompt, string filename)
        {
            return TextToImageFileInternal(prompt, negativePrompt, filename, new DiffuserConfig());
        }

        public Task<bool> TextToImageFile(string prompt, string filename, DiffuserConfig diffuserConfig)
        {
            return TextToImageFileInternal(prompt, null, filename, diffuserConfig);
        }

        public Task<bool> TextToImageFile(string prompt, string negativePrompt, string filename, DiffuserConfig diffuserConfig)
        {
            return TextToImageFileInternal(prompt, negativePrompt, filename, diffuserConfig);
        }



        private async Task<Image<Rgba32>> TextToImageInternal(string prompt, string negativePrompt, DiffuserConfig diffuserConfig)
        {
            return await Task.Run(() =>
            {
                var imageTensorData = _inferenceService.RunInference(prompt, negativePrompt, diffuserConfig);
                return _imageService.TensorToImage(imageTensorData, _configuration.Width, _configuration.Height);
            }).ConfigureAwait(false);
        }

        private async Task<bool> TextToImageFileInternal(string prompt, string negativePrompt, string filename, DiffuserConfig diffuserConfig)
        {
            var image = await TextToImageInternal(prompt, negativePrompt, diffuserConfig);
            if (image is null)
                return false;

            await image.SaveAsync(filename);
            return true;
        }

        public void Dispose()
        {
            _inferenceService.Dispose();
        }


    }
}
