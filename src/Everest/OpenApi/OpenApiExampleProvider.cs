using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace Everest.OpenApi
{
    public interface IOpenApiExampleProvider
    {
        string MimeType { get; }
        
        string GetExample();
    }

    public abstract class PlainExampleProvider<T> : IOpenApiExampleProvider
    {
        public string MimeType => "text/plain";

        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();
            return example.ToString();
        }

        protected abstract T GetExample();
    }

    public abstract class JsonExampleProvider<T> : IOpenApiExampleProvider
    {
        public string MimeType => "application/json";


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

    public abstract class XmlExampleProvider<T> : IOpenApiExampleProvider
    {
        public string MimeType => "application/xml";
        
        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, example);
                return writer.ToString();
            }
        }

        protected abstract T GetExample();
    }
}
