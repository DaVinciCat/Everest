using Everest.Http;

namespace Everest.EndPoints
{
	public interface IEndPointInvoker
	{
		bool TryInvokeEndPoint(HttpContext context);
	}
}
