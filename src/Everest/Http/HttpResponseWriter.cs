using System.IO;
using System.Threading.Tasks;

namespace Everest.Http
{
    public class HttpResponseWriter
    {
        private readonly HttpResponse response;

        public HttpResponseWriter(HttpResponse response)
        {
            this.response = response;
        }

        public virtual async Task Write(byte[] content)
        {
            response.ContentLength64 = content.Length;
            await response.OutputStream.WriteAsync(content, 0, content.Length);
        }

        public virtual async Task Write(Stream stream)
        {
            response.ContentLength64 = stream.Length;

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var buffer = new byte[4096];
            int read;

            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await response.OutputStream.WriteAsync(buffer, 0, read);
            }
        }
    }
}
