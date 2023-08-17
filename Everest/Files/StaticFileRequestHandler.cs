using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Media;
using System.Net;
using Everest.Utils;

namespace Everest.Files
{
	public class StaticFileRequestHandler : IStaticFileRequestHandler
	{
		#region Logger

		public ILogger<StaticFileRequestHandler> Logger { get; }

		#endregion
		
		#region Mime

		public string DefaultMimeType { get; set; } = "application/octet-stream";

		public IReadOnlyDictionary<string, MimeDescriptor> MimeTypes => new ReadOnlyDictionary<string, MimeDescriptor>(mimeTypes);

		private readonly IDictionary<string, MimeDescriptor> mimeTypes = new Dictionary<string, MimeDescriptor>
		{
			{".asf", new MimeDescriptor(Media.MimeTypes.Asf)},
			{".asx", new MimeDescriptor(Media.MimeTypes.Asx)},
			{".avi", new MimeDescriptor(Media.MimeTypes.Avi)},
			{".bin", new MimeDescriptor(Media.MimeTypes.Bin)},
			{".cco", new MimeDescriptor(Media.MimeTypes.Cco)},
			{".crt", new MimeDescriptor(Media.MimeTypes.Crt)},
			{".css", new MimeDescriptor(Media.MimeTypes.Css)},
			{".deb", new MimeDescriptor(Media.MimeTypes.Deb)},
			{".der", new MimeDescriptor(Media.MimeTypes.Der)},
			{".dll", new MimeDescriptor(Media.MimeTypes.Dll)},
			{".dmg", new MimeDescriptor(Media.MimeTypes.Dmg)},
			{".ear", new MimeDescriptor(Media.MimeTypes.Ear)},
			{".eot", new MimeDescriptor(Media.MimeTypes.Eot)},
			{".exe", new MimeDescriptor(Media.MimeTypes.Exe)},
			{".flv", new MimeDescriptor(Media.MimeTypes.Flv)},
			{".gif", new MimeDescriptor(Media.MimeTypes.Gif)},
			{".hqx", new MimeDescriptor(Media.MimeTypes.Hqx)},
			{".htc", new MimeDescriptor(Media.MimeTypes.Htc)},
			{".htm", new MimeDescriptor(Media.MimeTypes.Htm)},
			{".html", new MimeDescriptor(Media.MimeTypes.Html)},
			{".ico", new MimeDescriptor(Media.MimeTypes.Ico)},
			{".img", new MimeDescriptor(Media.MimeTypes.Img)},
			{".iso", new MimeDescriptor(Media.MimeTypes.Iso)},
			{".jar", new MimeDescriptor(Media.MimeTypes.Jar)},
			{".jardiff", new MimeDescriptor(Media.MimeTypes.Jardiff)},
			{".jng", new MimeDescriptor(Media.MimeTypes.Jng)},
			{".jnlp", new MimeDescriptor(Media.MimeTypes.Jnlp)},
			{".jpeg", new MimeDescriptor(Media.MimeTypes.Jpeg)},
			{".jpg", new MimeDescriptor(Media.MimeTypes.Jpg)},
			{".js", new MimeDescriptor(Media.MimeTypes.Js)},
			{".mml", new MimeDescriptor(Media.MimeTypes.Mml)},
			{".mng", new MimeDescriptor(Media.MimeTypes.Mng)},
			{".mov", new MimeDescriptor(Media.MimeTypes.Mov)},
			{".mp3", new MimeDescriptor(Media.MimeTypes.Mp3)},
			{".mpeg", new MimeDescriptor(Media.MimeTypes.Mpeg)},
			{".mpg", new MimeDescriptor(Media.MimeTypes.Mpg)},
			{".msi", new MimeDescriptor(Media.MimeTypes.Msi)},
			{".msm", new MimeDescriptor(Media.MimeTypes.Msm)},
			{".msp", new MimeDescriptor(Media.MimeTypes.Msp)},
			{".pdb", new MimeDescriptor(Media.MimeTypes.Pdb)},
			{".pdf", new MimeDescriptor(Media.MimeTypes.Pdf)},
			{".pem", new MimeDescriptor(Media.MimeTypes.Pem)},
			{".pl", new MimeDescriptor(Media.MimeTypes.Pl)},
			{".pm", new MimeDescriptor(Media.MimeTypes.Pm)},
			{".png", new MimeDescriptor(Media.MimeTypes.Png)},
			{".prc", new MimeDescriptor(Media.MimeTypes.Prc)},
			{".ra", new MimeDescriptor(Media.MimeTypes.Ra)},
			{".rar", new MimeDescriptor(Media.MimeTypes.Rar)},
			{".rpm", new MimeDescriptor(Media.MimeTypes.Rpm)},
			{".rss", new MimeDescriptor(Media.MimeTypes.Rss)},
			{".run", new MimeDescriptor(Media.MimeTypes.Run)},
			{".sea", new MimeDescriptor(Media.MimeTypes.Sea)},
			{".shtml", new MimeDescriptor(Media.MimeTypes.Shtml)},
			{".sit", new MimeDescriptor(Media.MimeTypes.Sit)},
			{".swf", new MimeDescriptor(Media.MimeTypes.Swf)},
			{".tcl", new MimeDescriptor(Media.MimeTypes.Tcl)},
			{".tk", new MimeDescriptor(Media.MimeTypes.Tk)},
			{".txt", new MimeDescriptor(Media.MimeTypes.Txt)},
			{".war", new MimeDescriptor(Media.MimeTypes.War)},
			{".wbmp", new MimeDescriptor(Media.MimeTypes.Wbmp)},
			{".wmv",new MimeDescriptor(Media.MimeTypes.Wmv)},
			{".xml", new MimeDescriptor(Media.MimeTypes.Xml)},
			{".xpi", new MimeDescriptor(Media.MimeTypes.Xpi)},
			{".zip", new MimeDescriptor(Media.MimeTypes.Zip)},
		};

		public void AddMimeType(string fileExtension, MimeDescriptor descriptor)
		{
			mimeTypes[fileExtension] = descriptor;
		}

		public void AddMimeType(string fileExtension, string contentType, string contentDisposition)
		{
			mimeTypes[fileExtension] = new MimeDescriptor(contentType, contentDisposition);
		}

		public void AddMimeType(string fileExtension, string contentType)
		{
			mimeTypes[fileExtension] = new MimeDescriptor(contentType);
		}

		public void RemoveMimeType(string fileExtension)
		{
			if (mimeTypes.ContainsKey(fileExtension))
			{
				mimeTypes.Remove(fileExtension);
			}
		}

		public void ClearMimeTypes()
		{
			mimeTypes.Clear();
		}

		#endregion

		#region Files

		private readonly IStaticFilesProvider filesProvider;

		#endregion

		#region Ctor
		
		public StaticFileRequestHandler(IStaticFilesProvider filesProvider, ILogger<StaticFileRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.filesProvider = filesProvider ?? throw new ArgumentNullException(nameof(filesProvider));
		}

		#endregion

		#region Handle
		
		public async Task<bool> TryServeStaticFileAsync(HttpContext context)
		{
			if (!context.Request.IsGetMethod() && !context.Request.IsHeadMethod())
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to serve file. Request method not supported: {new { Request = context.Request.Description }}");
				return false;
			}

			if (!filesProvider.TryGetFile(context.Request, out var file))
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

			this.mimeTypes.TryGetValue(file.Extension, out var mime);
			if (mime == null)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Unsupported mime type: {new { FileExtension = file.Extension, DefaultMimeType = DefaultMimeType }}");
				mime = new MimeDescriptor(DefaultMimeType);
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
