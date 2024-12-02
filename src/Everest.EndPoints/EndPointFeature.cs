using System;
using Everest.Collections;
using Everest.Http;

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

			return FeatureCollectionExtensions.Get<IEndPointFeature>(context.Features)?.EndPoint ?? throw new ArgumentNullException(nameof(IEndPointFeature));
		}

		public static bool TryGetEndPoint(this IHttpContext context, out EndPoint endPoint)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

            endPoint = FeatureCollectionExtensions.Get<IEndPointFeature>(context.Features)?.EndPoint;
			return endPoint != null;
		}
	}
}
