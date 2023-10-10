namespace Everest.OpenApi.Examples
{
    public abstract class PlainExampleProvider<T> : IOpenApiExampleProvider
    {
        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();
            return example.ToString();
        }

        protected abstract T GetExample();
    }
}
