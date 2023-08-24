using Microsoft.Extensions.Hosting;

namespace LLamaStack.Core.Services
{
    public class ModelLoaderService : IHostedService
    {
        private readonly IModelService _modelService;

        public ModelLoaderService(IModelService modelService)
        {
            _modelService = modelService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _modelService.LoadModels();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _modelService.UnloadModels();
        }
    }
}
