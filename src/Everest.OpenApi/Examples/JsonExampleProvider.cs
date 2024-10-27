using System.Text.Json;

namespace Everest.OpenApi.Examples
{
    public abstract class JsonExampleProvider<T> : IOpenApiExampleProvider
    {
        private readonly JsonSerializerOptions options;

        protected JsonExampleProvider(JsonSerializerOptions options)
        {
            this.options = options;
        }

        protected JsonExampleProvider()
        {

        }

        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();

            return options != null ?
                JsonSerializer.Serialize(example, options) :
                JsonSerializer.Serialize(example);
        }

        protected abstract T GetExample();
    }
}
