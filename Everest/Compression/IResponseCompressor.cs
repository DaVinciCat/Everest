using System;
using System.IO;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		public string[] Encodings { get; }

		void AddCompression(string encoding, Func<Stream, Stream> compression);

		void RemoveCompression(string encoding);

		void ClearCompressions();

		Task<bool> TryCreateResponseCompressionAsync(HttpContext context);
	}
}
