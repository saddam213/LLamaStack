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
            return TextToImageInternal(prompt,  new DiffuserConfig());
        }


        public Task<Image<Rgba32>> TextToImage(string prompt, DiffuserConfig diffuserConfig)
        {
            return TextToImageInternal(prompt,  diffuserConfig);
        }


        public Task<bool> TextToImageFile(string prompt, string filename)
        {
            return TextToImageFileInternal(prompt, filename,  new DiffuserConfig());
        }


        public Task<bool> TextToImageFile(string prompt, string filename, DiffuserConfig diffuserConfig)
        {
            return TextToImageFileInternal(prompt, filename,  diffuserConfig);
        }



        private async Task<Image<Rgba32>> TextToImageInternal(string prompt, DiffuserConfig diffuserConfig)
        {
            return await Task.Run(() =>
            {
                var imageTensorData = _inferenceService.RunInference(prompt, diffuserConfig);
                return _imageService.TensorToImage(imageTensorData, _configuration.Width, _configuration.Height);
            }).ConfigureAwait(false);
        }

        private async Task<bool> TextToImageFileInternal(string prompt, string filename,  DiffuserConfig diffuserConfig)
        {
            var image = await TextToImageInternal(prompt, diffuserConfig);
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
