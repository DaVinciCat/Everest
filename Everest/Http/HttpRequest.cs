﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Collections;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
	public class HttpRequest
	{
		public ILogger<HttpRequest> Logger { get; }

		public Guid TraceIdentifier => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasPayload => request.HasEntityBody;

		public string Path => request.Url?.AbsolutePath.TrimEnd('/');

		public IPEndPoint RemoteEndPoint => request.RemoteEndPoint;

		public string Description => $"{HttpMethod} {Path}";

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParameterCollection QueryParameters { get; set; }

		public ParameterCollection PathParameters { get; set; }

		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerContext context, ILogger<HttpRequest> logger)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			request = context.Request;

			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			QueryParameters = new ParameterCollection(request.QueryString);
			PathParameters = new ParameterCollection();
		}

		#region RequestData

		/*https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest.inputstream?view=net-7.0*/
		public async Task<string> ReadRequestDataAsync()
		{
			if (!request.HasEntityBody)
				return null;

			if (requestData != null)
				return requestData;

			await using (request.InputStream)
			{
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
				{
					requestData = await reader.ReadToEndAsync();
				}
			}

			return requestData;
		}

		private string requestData;

		#endregion
	}
}
