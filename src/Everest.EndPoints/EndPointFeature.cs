using System;
using Everest.Common.Collections;
using Everest.Core.Http;

namespace Everest.EndPoints
{
	public interface IEndPointFeature
	{
		EndPoint EndPoint { get; }
	}

	public class EndPointFeature : IEndPointFeature
	{
		public EndPoint EndPoint { get; }

		public EndPointFeature(EndPoint endPoint)
		{
            EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
		}
	}

	public static class HttpContextEndPointFeatureExtensions
	{
		public static EndPoint GetEndPoint(this IHttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			return context.Features.Get<IEndPointFeature>()?.EndPoint ?? throw new ArgumentNullException(nameof(IEndPointFeature));
		}

		public static bool TryGetEndPoint(this IHttpContext context, out EndPoint endPoint)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

            endPoint = context.Features.Get<IEndPointFeature>()?.EndPoint;
			return endPoint != null;
		}
	}
}
