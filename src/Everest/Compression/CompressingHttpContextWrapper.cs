using Everest.Http;

namespace Everest.Compression
{
    public sealed class CompressingHttpContextWrapper : HttpContextWrapper
    {
        public override IHttpResponse Response { get; }

        public CompressingHttpContextWrapper(IHttpContext context, IResponseCompressor compressor)
            : base(context)
        {
            Response = new CompressingHttpResponseWrapper(context, compressor);
        }
    }
}
