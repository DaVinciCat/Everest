using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		public Dictionary<string, Func<Stream, Stream>> Compressors { get; set; }

		Task<bool> TryCompressResponseAsync(HttpContext context);
	}
}
