using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Everest.Authentication;
using Everest.Builder;
using Everest.Compression;
using Everest.Core.Http;
using Everest.Core.Rest;
using Everest.Core.WebSockets;
using Everest.Cors;
using Everest.EndPoints;
using Everest.Exceptions;
using Everest.Logging;
using Everest.OpenApi;
using Everest.OpenApi.Annotations;
using Everest.OpenApi.Examples;
using Everest.Routing;
using Everest.StaticFiles;
using Everest.Swagger;
using Everest.SwaggerUi;
using Everest.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Shell
{
    [RestResource]
    [RoutePrefix("/api/1.0/tests")]
    public static class Rest
    {
        //[HttpGet("/")]
        //[RoutePrefix("/")]
        //public static async Task GetDefaultRoute(IHttpContext context)
        //{
        //	await context.Response.WriteTextAsync("default-route");
        //}

        //[HttpGet("/get/simple-route")]
        //public static async Task GetSimpleRoute(IHttpContext context)
        //{
        //          await context.Response.WriteTextAsync("simple-route");
        //      }

        //[HttpGet("/get/route-with-mandatory-query-parameter-p")]
        //public static async Task GetRouteWithMandatoryQueryParameterP(IHttpContext context)
        //{
        //	var p = context.Request.QueryParameters.GetParameterValue<string>("p");
        //          await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-p\n");
        //          await context.Response.WriteTextAsync($"p:{p}");
        //      }

        //      [HttpGet("/get/route-with-optional-query-parameter-p")]
        //      public static async Task GetRouteWithOptionalQueryParameterP(IHttpContext context)
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
        //      public static async Task GetRouteWithMandatoryQueryParameterOfTypeIntP(IHttpContext context)
        //      {
        //          var p = context.Request.QueryParameters.GetParameterValue<int>("p");
        //          await context.Response.WriteTextAsync("route-with-mandatory-query-parameter-of-type-int-p\n");
        //          await context.Response.WriteTextAsync($"p:{p}");
        //      }

        //      [HttpGet("/get/route-with-path-parameter-p/{p:string}")]
        //public static async Task GetRouteWithPathParameterP(IHttpContext context)
        //{
        //	    var p = context.Request.PathParameters.GetParameterValue<string>("p");
        //          await context.Response.WriteTextAsync("route-with-path-parameter-p/{p:string}\n");
        //          await context.Response.WriteTextAsync($"p:{p}");
        //      }

        [HttpGet("/get/route-with-path-parameter-of-type-int-p/{p:int}")]
        public static async Task GetRouteWithPathParameterOfTypeIntP(IHttpContext context)
        {
            var p = context.Request.PathParameters.GetParameterValue<int>("p");
            await context.Response.SendTextResponseAsync($"route-with-path-parameter-of-type-int-p/{p:int}{Environment.NewLine}p:{p}");
        }

        //      [HttpGet("/get/route-with-path-parameter-of-type-guid-p/{p:guid}")]
        //      public static async Task GetRouteWithPathParameterOfTypeGuidP(IHttpContext context)
        //      {
        //          var p = context.Request.PathParameters.GetParameterValue<Guid>("p");
        //          await context.Response.WriteTextAsync("route-with-path-parameter-of-type-guid-p/{p:guid}\n");
        //          await context.Response.WriteTextAsync($"p:{p}");
        //      }

        //[HttpOptions("/options/cors-request-response")]
        //public static async Task OptionsCorsRequestResponse(IHttpContext context)
        //{
        //	context.Response.AddHeader(HttpHeaders.Origin, "*");
        //	context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, string.Join(" ", HttpHeaders.ContentType, HttpHeaders.Accept, HttpHeaders.XRequestedWith));
        //	context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, string.Join(" ", HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete));
        //	context.Response.StatusCode = HttpStatusCode.NoContent;
        //	await context.Response.SendAsync();
        //}

        //[HttpGet("/get/compressed-response")]
        //public static async Task GetCompressedResponse(IHttpContext context)
        //{
        //          await context.Response.WriteTextAsync("compressed-response\n");
        //          await context.Response.WriteTextAsync(string.Concat(Enumerable.Repeat("text", 10000)));
        //}

        //[HttpGet("/get/send-response")]
        //public static async Task GetSendResponse(IHttpContext context)
        //{
        //          await context.Response.WriteTextAsync("send-response");
        //	await context.Response.SendAsync();
        //}

        //[HttpGet("/get/empty-response")]
        //public static async Task GetEmptyResponse(IHttpContext context)
        //{
        //	await context.Response.SendAsync();
        //}

        //[HttpGet("/get/exception-response")]
        //public static Task GetExceptionResponse(IHttpContext context)
        //{
        //	throw new InvalidOperationException("something went wrong ;(");
        //}

        [HttpPost("/get/request-with-text-payload")]
        public static async Task GetRequestWithTextPayload(IHttpContext context)
        {
            var payload = await context.Request.ReadRequestBodyAsTextAsync();
            await context.Response.SendTextResponseAsync($"request-with-text-payload{Environment.NewLine}payload:{payload}");
        }

        //[HttpGet("/get/request-with-json-payload")]
        //public static async Task GetRequestWithJsonPayload(IHttpContext context)
        //{
        //	var payload = await context.Request.ReadJsonAsync<object>();
        //          await context.Response.WriteTextAsync("request-with-json-payload\n");
        //          await context.Response.WriteTextAsync("payload:\n");
        //          await context.Response.WriteJsonAsync(payload);
        //}

        //      [HttpPost("/post/request-with-form-payload")]
        //      public static async Task PostRequestWithFormPayload(IHttpContext context)
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
        public static async Task GetGreetingsRequest(IHttpContext context)
        {
            var service = context.GetGreetingsService();
            var greetings = service.Greet();

            context.Request.PathParameters.TryGetParameterValue<string>("to", out var to);
            await context.Response.SendJsonResponseAsync(new { Message = greetings, From = "Everest", To = to, Success = true });
        }

        [HttpGet("/get/open-api-example/{id:guid}/{name:string}/{value:int}")]
        [Operation(Summary = "Operation summary", Description = "Operation description")]
        [PathParameter("id", Description = "Some int array parameter description")]
        [Tags("OpenApi", "Examples")]
        public static async Task GetOpenApiExample1(IHttpContext context)
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
        [QueryParameter("enum", typeof(EnumExample), Description = "Some enum parameter description")]
        [QueryParameter("int-param", typeof(int), Description = "Some int parameter description")]
        [QueryParameter("int-array", typeof(int[]), Description = "Some int array parameter description")]
        [QueryParameter("array-array", typeof(Array), Description = "Some int array parameter description")]
        [QueryParameter("int-enumerable", typeof(IEnumerable<int>), Description = "Some enum parameter description")]
        [QueryParameter("enumerable", typeof(IEnumerable), Description = "Some enum parameter description")]
        [QueryParameter("object-test", typeof(HashSet<int>), Description = "Some enum parameter description")]
        [QueryParameter("b-array", typeof(B[]), Description = "Some enum parameter description")]
        [QueryParameter("b-enumerable", typeof(IEnumerable<B>), Description = "Some enum parameter description")]
        [QueryParameter("b-hashset", typeof(HashSet<B>), Description = "Some enum parameter description")]
        public static async Task PostOpenApiExample(IHttpContext context)
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
                    .AddWebSocketRequestHandler<EchoWebsocketRequestHandler>()
                    .AddOpenApiDocumentGenerator()
                    .AddSwaggerEndPointGenerator(configurator => configurator.UseOpenApiInfo(info =>
                    {
                        info.Title = "Everest API";
                        info.Description = "Some API description";
                        info.Version = "V3";
                    }))
                    .AddSwaggerUiGenerator()
                    .AddSingleton(_ => new GreetingsService())
                    .AddConsoleLoggerFactory();

            var rest = RestServerBuilderFactory.CreateBuilder(services)
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
                .GenerateSwaggerEndPoint()
                .GenerateSwaggerUi()
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
                await SendAsync(session, $"session {session.Id}", CancellationToken.None);
                await BroadcastAsync(message, CancellationToken.None);
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
        public static GreetingsService GetGreetingsService(this IHttpContext context)
        {
            return context.Services.GetRequiredService<GreetingsService>();
        }
    }

    #endregion
}