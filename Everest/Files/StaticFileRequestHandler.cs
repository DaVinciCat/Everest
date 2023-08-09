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

		public string DefaultMimeType => "application/octet-stream";

		public IReadOnlyDictionary<string, MimeDescriptor> Mimes => new ReadOnlyDictionary<string, MimeDescriptor>(mimes);

		private readonly IDictionary<string, MimeDescriptor> mimes = new Dictionary<string, MimeDescriptor>
		{
			{".asf", new MimeDescriptor(MimeTypes.Asf)},
			{".asx", new MimeDescriptor(MimeTypes.Asx)},
			{".avi", new MimeDescriptor(MimeTypes.Avi)},
			{".bin", new MimeDescriptor(MimeTypes.Bin)},
			{".cco", new MimeDescriptor(MimeTypes.Cco)},
			{".crt", new MimeDescriptor(MimeTypes.Crt)},
			{".css", new MimeDescriptor(MimeTypes.Css)},
			{".deb", new MimeDescriptor(MimeTypes.Deb)},
			{".der", new MimeDescriptor(MimeTypes.Der)},
			{".dll", new MimeDescriptor(MimeTypes.Dll)},
			{".dmg", new MimeDescriptor(MimeTypes.Dmg)},
			{".ear", new MimeDescriptor(MimeTypes.Ear)},
			{".eot", new MimeDescriptor(MimeTypes.Eot)},
			{".exe", new MimeDescriptor(MimeTypes.Exe)},
			{".flv", new MimeDescriptor(MimeTypes.Flv)},
			{".gif", new MimeDescriptor(MimeTypes.Gif)},
			{".hqx", new MimeDescriptor(MimeTypes.Hqx)},
			{".htc", new MimeDescriptor(MimeTypes.Htc)},
			{".htm", new MimeDescriptor(MimeTypes.Htm)},
			{".html", new MimeDescriptor(MimeTypes.Html)},
			{".ico", new MimeDescriptor(MimeTypes.Ico)},
			{".img", new MimeDescriptor(MimeTypes.Img)},
			{".iso", new MimeDescriptor(MimeTypes.Iso)},
			{".jar", new MimeDescriptor(MimeTypes.Jar)},
			{".jardiff", new MimeDescriptor(MimeTypes.Jardiff)},
			{".jng", new MimeDescriptor(MimeTypes.Jng)},
			{".jnlp", new MimeDescriptor(MimeTypes.Jnlp)},
			{".jpeg", new MimeDescriptor(MimeTypes.Jpeg)},
			{".jpg", new MimeDescriptor(MimeTypes.Jpg)},
			{".js", new MimeDescriptor(MimeTypes.Js)},
			{".mml", new MimeDescriptor(MimeTypes.Mml)},
			{".mng", new MimeDescriptor(MimeTypes.Mng)},
			{".mov", new MimeDescriptor(MimeTypes.Mov)},
			{".mp3", new MimeDescriptor(MimeTypes.Mp3)},
			{".mpeg", new MimeDescriptor(MimeTypes.Mpeg)},
			{".mpg", new MimeDescriptor(MimeTypes.Mpg)},
			{".msi", new MimeDescriptor(MimeTypes.Msi)},
			{".msm", new MimeDescriptor(MimeTypes.Msm)},
			{".msp", new MimeDescriptor(MimeTypes.Msp)},
			{".pdb", new MimeDescriptor(MimeTypes.Pdb)},
			{".pdf", new MimeDescriptor(MimeTypes.Pdf)},
			{".pem", new MimeDescriptor(MimeTypes.Pem)},
			{".pl", new MimeDescriptor(MimeTypes.Pl)},
			{".pm", new MimeDescriptor(MimeTypes.Pm)},
			{".png", new MimeDescriptor(MimeTypes.Png)},
			{".prc", new MimeDescriptor(MimeTypes.Prc)},
			{".ra", new MimeDescriptor(MimeTypes.Ra)},
			{".rar", new MimeDescriptor(MimeTypes.Rar)},
			{".rpm", new MimeDescriptor(MimeTypes.Rpm)},
			{".rss", new MimeDescriptor(MimeTypes.Rss)},
			{".run", new MimeDescriptor(MimeTypes.Run)},
			{".sea", new MimeDescriptor(MimeTypes.Sea)},
			{".shtml", new MimeDescriptor(MimeTypes.Shtml)},
			{".sit", new MimeDescriptor(MimeTypes.Sit)},
			{".swf", new MimeDescriptor(MimeTypes.Swf)},
			{".tcl", new MimeDescriptor(MimeTypes.Tcl)},
			{".tk", new MimeDescriptor(MimeTypes.Tk)},
			{".txt", new MimeDescriptor(MimeTypes.Txt)},
			{".war", new MimeDescriptor(MimeTypes.War)},
			{".wbmp", new MimeDescriptor(MimeTypes.Wbmp)},
			{".wmv",new MimeDescriptor(MimeTypes.Wmv)},
			{".xml", new MimeDescriptor(MimeTypes.Xml)},
			{".xpi", new MimeDescriptor(MimeTypes.Xpi)},
			{".zip", new MimeDescriptor(MimeTypes.Zip)},
		};

		public void AddMimeDescriptor(string fileExtension, MimeDescriptor descriptor)
		{
			mimes[fileExtension] = descriptor;
		}

		public void RemoveMimeDescriptor(string fileExtension)
		{
			if (mimes.ContainsKey(fileExtension))
			{
				mimes.Remove(fileExtension);
			}
		}

		public void ClearMimeDescriptors()
		{
			mimes.Clear();
		}

		#endregion

		#region Files

		public IStaticFilesProvider StaticFilesProvider { get; }

		#endregion

		#region Ctor
		
		public StaticFileRequestHandler(IStaticFilesProvider staticFilesProvider, ILogger<StaticFileRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			StaticFilesProvider = staticFilesProvider ?? throw new ArgumentNullException(nameof(staticFilesProvider));
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

			if (!StaticFilesProvider.TryGetFile(context.Request, out var file))
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

			mimes.TryGetValue(file.Extension, out var mime);
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
