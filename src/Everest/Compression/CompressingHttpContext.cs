using Everest.Http;

namespace Everest.Compression
{
    public sealed class CompressingHttpContext : HttpContextProxy
    {
        public override IHttpResponse Response { get; }

        public CompressingHttpContext(IHttpContext context, IResponseCompressor compressor)
            : base(context)
        {
            Response = new CompressingHttpResponse(context, compressor);
        }
    }
}
