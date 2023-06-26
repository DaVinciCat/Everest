using Everest.Http;

namespace Everest.Response
{
	public interface IResponseSender
	{
		bool TrySendResponse(HttpContext context);
	}
}
