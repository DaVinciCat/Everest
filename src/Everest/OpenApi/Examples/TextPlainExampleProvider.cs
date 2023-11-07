using System;

namespace Everest.OpenApi.Examples
{
    public abstract class TextPlainExampleProvider<T> : IOpenApiExampleProvider
    {
        private readonly IFormatProvider provider;

        private readonly string format;

        protected TextPlainExampleProvider(IFormatProvider provider, string format)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.format = format ?? throw new ArgumentNullException(nameof(format));
        }
        
        protected TextPlainExampleProvider()
        {
           
        }

        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();


            if (provider == null && format == null)
                return example.ToString();


            return string.Format(provider, format, example);
        }

        protected abstract T GetExample();
    }
}
