using Everest.Http;
using Everest.Utils;

namespace Everest.Middleware
{
	public class CorsMiddleware : MiddlewareBase
	{
		public override void Invoke(HttpContext context)
		{
			context.Response.AddHeader("Access-Control-Allow-Origin", "*");
			context.Response.AddHeader("Access-Control-Allow-Headers", "X-Requested-With");

			if (HasNext)
				Next.Invoke(context);
		}
	}
}
