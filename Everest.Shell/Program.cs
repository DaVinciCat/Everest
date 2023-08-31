using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Everest.Authentication;
using Everest.Http;
using Everest.Rest;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Shell
{
	[RestResource]
	[RoutePrefix("/api/1.0/tests")]
	public static class Rest
	{
		[HttpGet("/")]
		[RoutePrefix("/")]
		public static async Task GetDefaultRoute(HttpContext context)
		{
			await context.Response.WriteTextAsync("default-route");
		}

		[HttpGet("/get/simple-route")]
		public static async Task GetSimpleRoute(HttpContext context)
		{
            await context.Response.WriteTextAsync("simple-route");
        }
		
		[HttpGet("/get/route-with-mandatory-query-parameter-p")]
		public static async Task GetRouteWithMandatoryQueryParameterP(HttpContext context)
		{
			var p = context.Request.QueryParameters.GetParameterValue<string>("p");
            await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-p\n");
            await context.Response.WriteTextAsync($"p:{p}");
        }

        [HttpGet("/get/route-with-optional-query-parameter-p")]
        public static async Task GetRouteWithOptionalQueryParameterP(HttpContext context)
        {
            if (!context.Request.QueryParameters.TryGetParameterValue<string>("p", out var p))
            {
                await context.Response.WriteTextAsync("route-with-optional-query-parameter-p\n");
                await context.Response.WriteTextAsync("no p");
            }
            else
            {
                await context.Response.WriteTextAsync("route-with-optional-query-parameter-p\n");
                await context.Response.WriteTextAsync($"p:{p}");
            }
        }

        [HttpGet("/get/route-with-mandatory-query-parameter-of-type-int-p")]
        public static async Task GetRouteWithMandatoryQueryParameterOfTypeIntP(HttpContext context)
        {
            var p = context.Request.QueryParameters.GetParameterValue<int>("p");
            await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-of-type-int-p\n");
            await context.Response.WriteTextAsync($"p:{p}");
        }

        [HttpGet("/get/route-with-path-parameter-p/{p:string}")]
		public static async Task GetRouteWithPathParameterP(HttpContext context)
		{
 		    var p = context.Request.PathParameters.GetParameterValue<string>("p");
            await context.Response.WriteTextAsync("route-with-path-parameter-p/{p:string}\n");
            await context.Response.WriteTextAsync($"p:{p}");
        }
		
		[HttpGet("/get/route-with-path-parameter-of-type-int-p/{p:int}")]
		public static async Task GetRouteWithPathParameterOfTypeIntP(HttpContext context)
		{
			var p = context.Request.PathParameters.GetParameterValue<int>("p");
            await context.Response.WriteTextAsync("route-with-path-parameter-of-type-int-p/{p:int}\n");
            await context.Response.WriteTextAsync($"p:{p}");
        }

        [HttpGet("/get/route-with-path-parameter-of-type-guid-p/{p:guid}")]
        public static async Task GetRouteWithPathParameterOfTypeGuidP(HttpContext context)
        {
            var p = context.Request.PathParameters.GetParameterValue<Guid>("p");
            await context.Response.WriteTextAsync("route-with-path-parameter-of-type-guid-p/{p:guid}\n");
            await context.Response.WriteTextAsync($"p:{p}");
        }

        [HttpOptions("/options/cors-request-response")]
		public static async Task OptionsCorsRequestResponse(HttpContext context)
		{
			context.Response.AddHeader(HttpHeaders.Origin, "*");
			context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, string.Join(" ", HttpHeaders.ContentType, HttpHeaders.Accept, HttpHeaders.XRequestedWith));
			context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, string.Join(" ", HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete));
			context.Response.StatusCode = HttpStatusCode.NoContent;
			await context.Response.SendAsync();
		}

		[HttpGet("/get/compressed-response")]
		public static async Task GetCompressedResponse(HttpContext context)
		{
            await context.Response.WriteTextAsync("compressed-response\n");
            await context.Response.WriteTextAsync(string.Concat(Enumerable.Repeat("text", 10000)));
		}

		[HttpGet("/get/send-response")]
		public static async Task GetSendResponse(HttpContext context)
		{
            await context.Response.WriteTextAsync("send-response");
			await context.Response.SendAsync();
		}

		[HttpGet("/get/empty-response")]
		public static async Task GetEmptyResponse(HttpContext context)
		{
			await context.Response.SendAsync();
		}

		[HttpGet("/get/exception-response")]
		public static Task GetExceptionResponse(HttpContext context)
		{
			throw new InvalidOperationException("something went wrong ;(");
		}

		[HttpGet("/get/request-with-text-payload")]
		public static async Task GetRequestWithTextPayload(HttpContext context)
		{
			var payload = await context.Request.ReadTextAsync();
            await context.Response.WriteTextAsync("request-with-text-payload\n");
            await context.Response.WriteTextAsync($"payload:{payload}");
        }
		
		[HttpGet("/get/request-with-json-payload")]
		public static async Task GetRequestWithJsonPayload(HttpContext context)
		{
			var payload = await context.Request.ReadJsonAsync<object>();
            await context.Response.WriteTextAsync("request-with-json-payload\n");
            await context.Response.WriteTextAsync("payload:\n");
            await context.Response.WriteJsonAsync(payload);
		}

        [HttpGet("/get/greetings-request/{to:string}")]
        public static async Task GetGreetingsRequest(HttpContext context)
        {
            var service = context.GetGreetingsService();
            var greetings = service.Greet();

            context.Request.PathParameters.TryGetParameterValue<string>("to", out var to);
            await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = to, Success = true });
            await context.Response.SendAsync();
        }
    }

	class Program
	{
		static void Main()
		{
			var services = new ServiceCollection();
			services.AddAuthenticator(configurator => configurator.AddBasicAuthentication())
					.AddCorsRequestHandler(configurator => configurator.AddDefaultCorsPolicy())
					.AddSingleton(_ => new GreetingsService())
					.AddConsoleLoggerFactory();

			using var rest = new RestServerBuilder(services)
				.UsePrefixes("http://localhost:8080/")
				.UseExceptionHandlerMiddleware()
				.UseResponseCompressionMiddleware()
                .UseStaticFilesMiddleware()
                .UseRoutingMiddleware()
                .UseCorsMiddleware()
				.UseAuthenticationMiddleware()
				.UseEndPointMiddleware()
				.ScanRoutes(Assembly.GetExecutingAssembly())
				.Build();

			rest.Start();

            Console.WriteLine(@"
 ______                       _
|  ____|                     | |  
| |____   _____ _ __ ___  ___| |_ 
|  __\ \ / / _ \ '__/ _ \/ __| __|
| |___\ V /  __/ | |  __/\__ \ |_ 
|______\_/ \___|_|  \___||___/\__|");

            Console.WriteLine("\nhttp://localhost:8080/");
            Console.WriteLine("\nPress any key to exit");
			Console.ReadKey();
		}
	}

	#region Greetings

	public class GreetingsService
	{
		public GreetingsService()
		{
			//for test purposes only
		}

		private readonly string[] greetings =
		{
			"Hello!",
			"Hi!",
			"Welcome!",
			"Howdy!",
			"Hi, buddy!",
			"What's up?",
			"Salute!"
		};

		public string Greet()
		{
			var rnd = new Random(DateTime.Now.Millisecond);
			var index = rnd.Next(0, greetings.Length - 1);

			return greetings[index];
		}
	}

	public static class HttpContextExtensions
	{
		public static GreetingsService GetGreetingsService(this HttpContext context)
		{
			return context.Services.GetRequiredService<GreetingsService>();
		}
	}

	#endregion
}