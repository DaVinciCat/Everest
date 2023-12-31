﻿using System;
using Everest.Collections;
using Everest.Features;
using Everest.Routing;

namespace Everest.Http
{
	public class HttpContext
	{
		public Guid Id { get; } = Guid.NewGuid();

		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public IFeatureCollection Features { get; }

		public IServiceProvider Services { get; }
		
		public HttpContext(HttpRequest request, HttpResponse response, IFeatureCollection features, IServiceProvider services)
		{
			Request = request;
			Response = response;
			Features = features;
			Services = services;
		}
	}

	public static class HttpContextExtensions
	{
		public static EndPoint GetEndPoint(this HttpContext context)
		{
			return context.Features.Get<IEndPointFeature>()?.EndPoint;
		}
	}
}
