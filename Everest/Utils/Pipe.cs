using System;
using System.IO;
using System.Threading.Tasks;

namespace Everest.Utils
{
	public class Pipe : IDisposable
	{
		public long Length => input.Length;

		private Stream input;

		private Stream output;

		public Pipe(Stream to)
		{
			if (to == null) 
				throw new ArgumentNullException(nameof(to));

			CheckOutputStream(to);

			input = new MemoryStream();
			output = to;
		}

		public Pipe PipeFrom(Stream from)
		{
			if (from == null) 
				throw new ArgumentNullException(nameof(from));

			CheckInputStream(from);

			input = from;
			return this;
		}

		public Pipe PipeFrom(Func<Stream, Stream> from)
		{
			if (from == null)
				throw new ArgumentNullException(nameof(from));

			var stream = from(input);
			CheckInputStream(stream);
			input = stream;

			return this;
		}

		public Pipe PipeTo(Func<Stream, Stream> to)
		{
			if (to == null) 
				throw new ArgumentNullException(nameof(to));

			var stream = to(output);
			CheckOutputStream(stream);
			output = stream;

			return this;
		}

		public Pipe PipeTo(Stream to)
		{
			if (to == null)
				throw new ArgumentNullException(nameof(to));

			CheckOutputStream(to);

			output = to;

			return this;
		}

		public async Task FlushAsync()
		{
			input.Position = 0;

			var buffer = new byte[4096];
			int read;
			
			while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				await output.WriteAsync(buffer, 0, read);
			}
		}

		#region Check

		private void CheckOutputStream(Stream to)
		{
			if (!to.CanWrite)
			{
				throw new InvalidOperationException("Output stream is not writeable");
			}
		}

		private void CheckInputStream(Stream from)
		{
			if (!from.CanRead)
			{
				throw new InvalidOperationException("Input stream is not readable");
			}

			if (!from.CanSeek)
			{
				throw new InvalidOperationException("Input stream is not seekable");
			}
		}

		#endregion

		#region IDisposable

		private bool disposed;

		public void Dispose()
		{
			if(disposed)
				return;

			input?.Dispose();
			output?.Dispose();

			disposed = true;
		}

		#endregion


	}
}
