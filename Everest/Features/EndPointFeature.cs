using Everest.Routing;

namespace Everest.Features
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
			EndPoint = endPoint;
		}
	}
}
