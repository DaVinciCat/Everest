using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Everest.Authentication;
using Everest.Http;
using Everest.OpenApi.Annotations;
using Everest.OpenApi.Examples;
using Everest.Rest;
using Everest.Routing;
using Everest.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Everest.Shell
{
	[RestResource]
	[RoutePrefix("/api/1.0/tests")]
	public static class Rest
	{
		//[HttpGet("/")]
		//[RoutePrefix("/")]
		//public static async Task GetDefaultRoute(HttpContext context)
		//{
		//	await context.Response.WriteTextAsync("default-route");
		//}

		//[HttpGet("/get/simple-route")]
		//public static async Task GetSimpleRoute(HttpContext context)
		//{
  //          await context.Response.WriteTextAsync("simple-route");
  //      }
		
		//[HttpGet("/get/route-with-mandatory-query-parameter-p")]
		//public static async Task GetRouteWithMandatoryQueryParameterP(HttpContext context)
		//{
		//	var p = context.Request.QueryParameters.GetParameterValue<string>("p");
  //          await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-p\n");
  //          await context.Response.WriteTextAsync($"p:{p}");
  //      }

  //      [HttpGet("/get/route-with-optional-query-parameter-p")]
  //      public static async Task GetRouteWithOptionalQueryParameterP(HttpContext context)
  //      {
  //          if (!context.Request.QueryParameters.TryGetParameterValue<string>("p", out var p))
  //          {
  //              await context.Response.WriteTextAsync("route-with-optional-query-parameter-p\n");
  //              await context.Response.WriteTextAsync("no p");
  //          }
  //          else
  //          {
  //              await context.Response.WriteTextAsync("route-with-optional-query-parameter-p\n");
  //              await context.Response.WriteTextAsync($"p:{p}");
  //          }
  //      }

  //      [HttpGet("/get/route-with-mandatory-query-parameter-of-type-int-p")]
  //      public static async Task GetRouteWithMandatoryQueryParameterOfTypeIntP(HttpContext context)
  //      {
  //          var p = context.Request.QueryParameters.GetParameterValue<int>("p");
  //          await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-of-type-int-p\n");
  //          await context.Response.WriteTextAsync($"p:{p}");
  //      }

  //      [HttpGet("/get/route-with-path-parameter-p/{p:string}")]
		//public static async Task GetRouteWithPathParameterP(HttpContext context)
		//{
 	//	    var p = context.Request.PathParameters.GetParameterValue<string>("p");
  //          await context.Response.WriteTextAsync("route-with-path-parameter-p/{p:string}\n");
  //          await context.Response.WriteTextAsync($"p:{p}");
  //      }
		
		//[HttpGet("/get/route-with-path-parameter-of-type-int-p/{p:int}")]
		//public static async Task GetRouteWithPathParameterOfTypeIntP(HttpContext context)
		//{
		//	var p = context.Request.PathParameters.GetParameterValue<int>("p");
  //          await context.Response.WriteTextAsync("route-with-path-parameter-of-type-int-p/{p:int}\n");
  //          await context.Response.WriteTextAsync($"p:{p}");
  //      }

  //      [HttpGet("/get/route-with-path-parameter-of-type-guid-p/{p:guid}")]
  //      public static async Task GetRouteWithPathParameterOfTypeGuidP(HttpContext context)
  //      {
  //          var p = context.Request.PathParameters.GetParameterValue<Guid>("p");
  //          await context.Response.WriteTextAsync("route-with-path-parameter-of-type-guid-p/{p:guid}\n");
  //          await context.Response.WriteTextAsync($"p:{p}");
  //      }

  //      [HttpOptions("/options/cors-request-response")]
		//public static async Task OptionsCorsRequestResponse(HttpContext context)
		//{
		//	context.Response.AddHeader(HttpHeaders.Origin, "*");
		//	context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, string.Join(" ", HttpHeaders.ContentType, HttpHeaders.Accept, HttpHeaders.XRequestedWith));
		//	context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, string.Join(" ", HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete));
		//	context.Response.StatusCode = HttpStatusCode.NoContent;
		//	await context.Response.SendAsync();
		//}

		//[HttpGet("/get/compressed-response")]
		//public static async Task GetCompressedResponse(HttpContext context)
		//{
  //          await context.Response.WriteTextAsync("compressed-response\n");
  //          await context.Response.WriteTextAsync(string.Concat(Enumerable.Repeat("text", 10000)));
		//}

		//[HttpGet("/get/send-response")]
		//public static async Task GetSendResponse(HttpContext context)
		//{
  //          await context.Response.WriteTextAsync("send-response");
		//	await context.Response.SendAsync();
		//}

		//[HttpGet("/get/empty-response")]
		//public static async Task GetEmptyResponse(HttpContext context)
		//{
		//	await context.Response.SendAsync();
		//}

		//[HttpGet("/get/exception-response")]
		//public static Task GetExceptionResponse(HttpContext context)
		//{
		//	throw new InvalidOperationException("something went wrong ;(");
		//}

		//[HttpGet("/get/request-with-text-payload")]
		//public static async Task GetRequestWithTextPayload(HttpContext context)
		//{
		//	var payload = await context.Request.ReadTextAsync();
  //          await context.Response.WriteTextAsync("request-with-text-payload\n");
  //          await context.Response.WriteTextAsync($"payload:{payload}");
  //      }
		
		//[HttpGet("/get/request-with-json-payload")]
		//public static async Task GetRequestWithJsonPayload(HttpContext context)
		//{
		//	var payload = await context.Request.ReadJsonAsync<object>();
  //          await context.Response.WriteTextAsync("request-with-json-payload\n");
  //          await context.Response.WriteTextAsync("payload:\n");
  //          await context.Response.WriteJsonAsync(payload);
		//}

  //      [HttpPost("/post/request-with-form-payload")]
  //      public static async Task PostRequestWithFormPayload(HttpContext context)
  //      {
  //          var payload = await context.Request.ReadFormAsync();
  //          await context.Response.WriteTextAsync("request-with-form-payload\n");
  //          await context.Response.WriteTextAsync("payload:\n");

  //          foreach (var key in payload.AllKeys)
  //          {
  //              await context.Response.WriteTextAsync($"{key}={payload[key]}\n");
  //          }
  //      }

  //      [HttpGet("/get/greetings-request/{to:string}")]
        public static async Task GetGreetingsRequest(HttpContext context)
        {
            var service = context.GetGreetingsService();
            var greetings = service.Greet();

            context.Request.PathParameters.TryGetParameterValue<string>("to", out var to);
            await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = to, Success = true });
        }

        [HttpGet("/get/open-api-example/{id:guid}/{name:string}/{value:int}")]
        [Operation(Summary = "Operation summary", Description = "Operation description")]
        [PathParameter("id", Description = "Some int array parameter description")]
        [Tags("OpenApi", "Examples")]
        public static async Task GetOpenApiExample1(HttpContext context)
        {

        }


        [HttpGet("/post/open-api-example")]
        [Operation(Summary = "Operation summary", Description = "Operation description")]
        [Tags("OpenApi", "Examples")]
        [RequestBody(typeof(Request), "application/json", "application/xml")]
        [RequestBodyExample(typeof(JsonRequestExample1), "application/json")]
        [RequestBodyExample(typeof(JsonRequestExample2), "application/json")]
        [RequestBodyExample(typeof(XmlRequestExample), "application/xml", Summary = "Xml")]
        [Response(HttpStatusCode.OK, typeof(List<B>), "application/json", "application/xml", Description = "Sdfsdfsfsdfsdf")]
        [Response(HttpStatusCode.BadRequest, "application/json", Description = "Sdfsdfsfsdfsdf")]
        [ResponseExample(HttpStatusCode.OK, typeof(JsonResponseExample1), "application/json")]
        [ResponseExample(HttpStatusCode.OK, typeof(JsonResponseExample2), "application/json")]
        [ResponseExample(HttpStatusCode.OK, typeof(XmlResponseExample), "application/xml")]
        [ResponseExample(HttpStatusCode.BadRequest, typeof(JsonResponseExample2), "application/json")]
        [QueryParameter("param", typeof(string), Description = "Some parameter description")]
        [QueryParameter("int-param", typeof(int), Description = "Some int parameter description")]
        [QueryParameter("int-array", typeof(int[]), Description = "Some int array parameter description")]
        [QueryParameter("enum", typeof(EnumExample), Description = "Some enum parameter description")]
        [QueryParameter("object-test", typeof(B), Description = "Some enum parameter description")]
        public static async Task PostOpenApiExample(HttpContext context)
        {
               
        }
    }

	class Program
	{
		static void Main()
		{
			var services = new ServiceCollection();
			services.AddAuthenticator(configurator => configurator.AddBasicAuthentication())
					.AddCorsRequestHandler(configurator => configurator.AddDefaultCorsPolicy())
                    .AddOpenApiDocumentGenerator(configurator => configurator.DocumentGenerator.OpenApiInfo = new OpenApiInfo { Title = "Everest", Version = "V3"})
                    .AddWebSocketRequestHandler<EchoWebsocketRequestHandler>()
					.AddSingleton(_ => new GreetingsService())
					.AddConsoleLoggerFactory();

			using var rest = new RestServerBuilder(services)
				.UsePrefixes("http://localhost:8080/")
				.UseExceptionHandlerMiddleware()
				.UseResponseCompressionMiddleware()
                .UseAuthenticationMiddleware()
                .UseWebSocketMiddleware<EchoWebsocketRequestHandler>()
                .UseStaticFilesMiddleware()
                .UseRoutingMiddleware()
                .UseCorsMiddleware()
				.UseEndPointMiddleware()
				.ScanRoutes(Assembly.GetExecutingAssembly())
                .UseSwagger()
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

    #region Examples

    public enum EnumExample
    {
        Test,
        Enum,
        Example
    }

    public class A
    {
        public string PropertyA { get; }


    }

    public class B
    {
        public string PropertyB { get; }

        public A A { get; }

        public IEnumerable<A> Enumerable { get; }

        public Dictionary<string, int> Dict { get; }
    }

    public class Request
    {
        public string Payload { get; }
        public Request()
        {

        }

        public Request(string payload)
        {
            Payload = payload;
        }
    }

    public class Response
    {
        public string Payload { get; }
        public Response()
        {

        }

        public Response(string payload)
        {
            Payload = payload;
        }
    }

    public class JsonRequestExample1 : JsonExampleProvider<Request>
    {
        protected override Request GetExample()
        {
            return new Request("payload #1");
        }
    }

    public class JsonRequestExample2 : JsonExampleProvider<Request>
    {
        protected override Request GetExample()
        {
            return new Request("payload #2");
        }
    }

    public class XmlRequestExample : XmlExampleProvider<Request>
    {
        protected override Request GetExample()
        {
            return new Request("payload #3");
        }
    }

    public class JsonResponseExample1 : JsonExampleProvider<Response>
    {
        protected override Response GetExample()
        {
            return new Response("payload #1");
        }
    }

    public class JsonResponseExample2 : JsonExampleProvider<Response>
    {
        protected override Response GetExample()
        {
            return new Response("payload #2");
        }
    }

    public class XmlResponseExample : XmlExampleProvider<Response>
    {
        protected override Response GetExample()
        {
            return new Response("payload #3");
        }
    }

    #endregion

    #region Sockets

    public class EchoWebsocketRequestHandler : WebSocketRequestHandler
    {
        public EchoWebsocketRequestHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<EchoWebsocketRequestHandler>())
        {

        }

        protected override async Task OnMessageAsync(WebSocketSession session, string message)
        {
			Logger.LogInformation($"Received WebSocket message: {message}");
			await Echo();

			async Task Echo()
            {
                await BroadcastAsync(message);
            }
        }

        public EchoWebsocketRequestHandler(ILogger logger) 
            : base(logger)
        {
        }
    }

    #endregion

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