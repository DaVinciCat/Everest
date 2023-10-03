﻿using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Everest.Http;
using System.Net;
using Everest.Mime;
using Everest.Utils;

namespace Everest.Files
{
	public class StaticFileRequestHandler : IStaticFileRequestHandler
	{
		#region Logger

		public ILogger<StaticFileRequestHandler> Logger { get; }

		#endregion

		#region Files
		
		private readonly IStaticFilesProvider staticFilesProvider;

		#endregion

		#region Mime

		private readonly IMimeTypesProvider mimeTypesProvider;

		public string UnknownContentType { get; set; } = "application/octet-stream";

		#endregion

		#region Ctor

		public StaticFileRequestHandler(IStaticFilesProvider staticFilesProvider, IMimeTypesProvider mimeTypesProvider, ILogger<StaticFileRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.staticFilesProvider = staticFilesProvider ?? throw new ArgumentNullException(nameof(staticFilesProvider));
			this.mimeTypesProvider = mimeTypesProvider ?? throw new ArgumentNullException(nameof(mimeTypesProvider));
		}

		#endregion

		#region Handle
		
        public async Task<bool> TryServeStaticFileAsync(HttpContext context)
        {
            var filePath = context.Request.RequestPathToFilePath();
            if (!staticFilesProvider.TryGetFile(filePath, out var file))
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

			mimeTypesProvider.TryGetMimeType(file.Extension, out var mimeType);
			if (mimeType == null)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Unsupported mime type: {new { FileExtension = file.Extension }}");
				mimeType = new MimeType(file.Extension, UnknownContentType, true);
			}
			
			var descriptor = new StaticFileDescriptor(file, mimeType);
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
			await context.Response.WriteFileAsync(descriptor.FileInfo.FullName, descriptor.MimeType.ContentType.MediaType, descriptor.MimeType.ContentDisposition.DispositionType);
			return true;
		};

		#endregion
	}
}