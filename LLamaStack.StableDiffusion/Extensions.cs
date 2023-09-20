using Microsoft.ML.OnnxRuntime;

namespace LLamaStack.StableDiffusion
{
    internal static class Extensions
    {
        public static T FirstElementAs<T>(this IDisposableReadOnlyCollection<DisposableNamedOnnxValue> collection)
        {
            if (collection is null || collection.Count == 0)
                return default;

            var element = collection.FirstOrDefault();
            if (element.Value is not T value)
                return default;

            return value;
        }
    }
}
