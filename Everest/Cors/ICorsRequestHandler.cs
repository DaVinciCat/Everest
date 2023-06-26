using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		public bool TryHandleCorsRequest(HttpContext context);
	}
}
