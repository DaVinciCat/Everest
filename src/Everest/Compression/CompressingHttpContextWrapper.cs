using Everest.Http;

namespace Everest.Compression
{
    public sealed class CompressingHttpContextWrapper : HttpContextWrapper
    {
        public override IHttpResponse Response { get; }

        public CompressingHttpContextWrapper(IHttpContext context, IResponseCompressorProvider provider)
            : base(context)
        {
            Response = new CompressingHttpResponseWrapper(context, provider);
        }
    }
}
