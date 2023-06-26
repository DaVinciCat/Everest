using System;
using Everest.Converters;
using Everest.Http;
using Everest.Net;
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

		public bool TrySendResponse(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try send response to: {context.Request.RemoteEndPoint.Description()}");
			
			if (context.Response.IsSent)
			{
				Logger.LogWarning($"{context.Id} - Failed to send response. Response is already sent");
				return false;
			}

			if (context.Response.IsClosed)
			{
				Logger.LogWarning($"{context.Id} - Failed to send response. Response is closed");
				return false;
			}

			context.Response.Send();
			Logger.LogTrace($"{context.Id} - Successfully sended response to: {context.Request.RemoteEndPoint.Description()} [{context.Response.Body.ToReadableSize()}]");
			return true;
		}
	}
}
