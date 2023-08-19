using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Everest.Http;
using System.Net;
using Everest.Media;
using Everest.Utils;

namespace Everest.Files
{
	public class StaticFileRequestHandler : IStaticFileRequestHandler
	{
		#region Logger

		public ILogger<StaticFileRequestHandler> Logger { get; }

		#endregion

		#region Files
		
		private readonly IStaticFilesProvider filesProvider;

		#endregion

		#region Mime

		private readonly IMimeProvider mimeProvider;

		public string UnknownContentType { get; set; } = "application/octet-stream";

		#endregion

		#region Ctor

		public StaticFileRequestHandler(IStaticFilesProvider filesProvider, IMimeProvider mimeProvider, ILogger<StaticFileRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.filesProvider = filesProvider ?? throw new ArgumentNullException(nameof(filesProvider));
			this.mimeProvider = mimeProvider ?? throw new ArgumentNullException(nameof(mimeProvider));
		}

		#endregion

		#region Handle
		
        public async Task<bool> TryServeStaticFileAsync(HttpContext context)
        {
            var physicalPath = filesProvider.GetFilePhysicalPath(context.Request);
            if (!filesProvider.TryGetFile(physicalPath, out var file))
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to serve file. Requested file not found: {new { RequestPath = context.Request.Path, Request = context.Request.Description }}");
				await OnFileNotFoundAsync(context);
				return false;
			}

			if (!file.Exists)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to serve file. Requested file does not exist: {new { PhysicalPath = file.FullName, Request = context.Request.Description }}");
				await OnFileNotFoundAsync(context);
				return false;
			}

			mimeProvider.TryGetMime(file.Extension, out var mime);
			if (mime == null)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Unsupported mime type: {new { FileExtension = file.Extension }}");
				mime = new Mime(file.Extension, UnknownContentType, true);
			}
			
			var descriptor = new StaticFileDescriptor(file, mime);
			var result = await OnServeFileAsync(context, descriptor);

			if (result)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - Successfully served requested file: {new { RequestPath = context.Request.Path, PhysicalPath = file.FullName, Size = file.Length.ToReadableSize() }}");
				return true;
			}

			return false;
		}

        public Func<HttpContext, Task> OnFileNotFoundAsync { get; set; } = async context =>
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			context.Response.KeepAlive = false;
			context.Response.StatusCode = HttpStatusCode.NotFound;
			context.Response.Clear();
			await context.Response.WriteTextAsync($"Requested file not found: {context.Request.Description}");
		};

		public Func<HttpContext, StaticFileDescriptor, Task<bool>> OnServeFileAsync { get; set; } = async (context, descriptor) =>
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			context.Response.StatusCode = HttpStatusCode.OK;
			await context.Response.WriteFileAsync(descriptor.File.FullName, descriptor.Mime.ContentType.MediaType, descriptor.Mime.ContentDisposition.DispositionType);
			return true;
		};

		#endregion
	}
}
