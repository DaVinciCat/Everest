using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Response
{
	public interface IResponseSender
	{
		Task<bool> TrySendResponseAsync(HttpContext context);
	}
}
