using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Net;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Response
{
	public class ResponseSender : IResponseSender
	{
		public ILogger<ResponseSender> Logger { get; }
		
		public ResponseSender(ILogger<ResponseSender> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<bool> TrySendResponseAsync(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to send response to: {context.Request.RemoteEndPoint.Description()}");
			await context.Response.SendResponceAsync();
			Logger.LogTrace($"{context.Id} - Successfully sended response to: {context.Request.RemoteEndPoint.Description()} [{context.Response.ContentLength64.ToReadableSize()}]");
			return true;
		}
	}
}
