using LLama.Abstractions;

namespace LLamaStack.Core.Config
{
    public interface IModelConfig : IModelParams
    {
        string Name { get; set; }
        int MaxInstances { get; set; }
    }
}
