using LLamaStack.Core.Config;

namespace LLamaStack.Core.Services
{
    public interface IModelService
    {
        Task<LLamaStackContext> CreateContext(string modelName, string key);
        Task<LLamaStackContext> GetContext(string modelName, string key);
        Task<LLamaStackModel> GetModel(string modelName);
        Task<LLamaStackModel> LoadModel(ModelConfig modelConfig);
        Task LoadModels();
        Task UnloadModel(string modelName);
        Task UnloadModels();
        Task<bool> RemoveContext(string modelName, string key);
        Task<LLamaStackContext> GetOrCreateModelAndContext(string modelName, string key);
    }
}