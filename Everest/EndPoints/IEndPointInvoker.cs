using System.Threading.Tasks;
using Everest.Http;

namespace Everest.EndPoints
{
	public interface IEndPointInvoker
	{
		Task<bool> TryInvokeEndPointAsync(HttpContext context);
	}
}
