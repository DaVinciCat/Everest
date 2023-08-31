using System;
using System.IO;
using System.Threading.Tasks;

namespace Everest.Utils
{
    public class StreamPipe : IDisposable
    {
        public long Length => input.Length;

        private Stream input;

        private Stream output;

        public StreamPipe(Stream to)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));

            CheckOutputStream(to);

            input = new MemoryStream();
            output = to;
        }

        public StreamPipe PipeFrom(Stream from)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            CheckInputStream(from);

            input = from;
            return this;
        }
        
		public StreamPipe PipeFrom(Func<Stream, Stream> from)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            var stream = from(input);
            CheckInputStream(stream);
            input = stream;

            return this;
        }

		public async Task<StreamPipe> PipeFromAsync(Task<Stream> from)
		{
			if (from == null)
				throw new ArgumentNullException(nameof(from));

			var stream = await from;
			CheckInputStream(stream);

			input = stream;
			return this;
		}

		public async Task<StreamPipe> PipeFromAsync(Func<Stream, Task<Stream>> from)
        {
			if (from == null)
				throw new ArgumentNullException(nameof(from));

			var stream = await from(input);
			CheckInputStream(stream);
			input = stream;

			return this;
		}

		public StreamPipe PipeTo(Stream to)
		{
			if (to == null)
				throw new ArgumentNullException(nameof(to));

			CheckOutputStream(to);

			output = to;

			return this;
		}

		public StreamPipe PipeTo(Func<Stream, Stream> to)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));

            var stream = to(output);
            CheckOutputStream(stream);
            output = stream;

            return this;
        }
        
        public async Task<StreamPipe> PipeToAsync(Task<Stream> to)
        {
			if (to == null)
				throw new ArgumentNullException(nameof(to));

			var stream = await to;
			CheckOutputStream(stream);

			output = stream;

			return this;
		}

        public async Task<StreamPipe> PipeToAsync(Func<Stream, Task<Stream>> to)
        {
			if (to == null)
				throw new ArgumentNullException(nameof(to));

			var stream = await to(output);
			CheckOutputStream(stream);
			output = stream;

			return this;
		}

        public StreamPipe Flush()
        {
	        if (input.CanSeek)
	        {
		        input.Position = 0;
	        }

	        var buffer = new byte[4096];
	        int read;

	        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
	        {
		        output.WriteAsync(buffer, 0, read);
	        }

	        return this;
        }

        public StreamPipe Close()
        {
            input.Close();
            output.Close();

            return this;
        }

		public async Task<StreamPipe> FlushAsync()
        {
	        if (input.CanSeek)
	        {
		        input.Position = 0;
	        }

            var buffer = new byte[4096];
            int read;

            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await output.WriteAsync(buffer, 0, read);
            }

            return this;
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
        }

        #endregion

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            if (disposed)
                return;

            input.Dispose();
            output.Dispose();

            disposed = true;
        }

        public async Task DisposeAsync()
        {
            if (disposed)
                return;

            await input.DisposeAsync();
            await output.DisposeAsync();

            disposed = true;
        }

        #endregion
    }
}
