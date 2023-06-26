using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		CorsPolicyCollection Policies { get; }
		
		public bool TryHandleCorsRequest(HttpContext context);
	}
}
