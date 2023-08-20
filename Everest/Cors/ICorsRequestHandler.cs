using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
