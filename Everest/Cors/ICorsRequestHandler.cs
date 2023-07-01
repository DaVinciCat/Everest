using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		CorsPolicyCollection Policies { get; }
		
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
