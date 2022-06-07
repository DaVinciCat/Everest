using System.Collections.Specialized;
using Everest.Routing;
using Xunit;

namespace Everest.Tests
{
	public class RoutingTests
	{
		[Fact]
		public void False_FooBar_Slash()
		{
			const string pattern = "/foo/bar";
			const string endPoint = "/";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, endPoint, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBar_FooBar()
		{
			const string pattern = "/foo/bar";
			const string endPoint = "/foo/bar";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);

			Assert.True(parser.TryParse(segment, endPoint, new NameValueCollection()));
		}

		[Fact]
		public void False_FooBar_Bar()
		{
			const string pattern = "/foo/bar";
			const string endPoint = "/bar";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, endPoint, new NameValueCollection()));
		}

		[Fact]
		public void False_FooBar_FooBaz()
		{
			const string pattern = "/foo/bar";
			const string endPoint = "/foo/baz";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, endPoint, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBarId_FooBar1()
		{
			const string pattern = "/foo/bar/{id}";
			const string endPoint = "/foo/bar/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}

		[Fact]
		public void True_FooId1BarId2_Foo1Bar2()
		{
			const string pattern = "/foo/{id1}/bar/{id2}";
			const string endPoint = "/foo/1/bar/2";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id1"] != null);
			Assert.True(parameters["id1"] == "1");

			Assert.True(parameters["id2"] != null);
			Assert.True(parameters["id2"] == "2");
		}

		[Fact]
		public void False_FooId1BarId2_Foo1()
		{
			const string pattern = "/foo/{id1}/bar/{id2}";
			const string endPoint = "/foo/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, endPoint, parameters));
		}

		[Fact]
		public void False_FooBar_FooBarQueryId()
		{
			const string pattern = "/foo/bar";
			const string endPoint = "/foo/bar?id=1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, endPoint, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBarIdInt_FooBar1()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string endPoint = "/foo/bar/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}

		[Fact]
		public void False_FooBarIdInt_FooBar1dot10()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string endPoint = "/foo/bar/1.10";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, endPoint, parameters));
		}

		[Fact]
		public void False_FooBarIdInt_FooBarAbc()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string endPoint = "/foo/bar/abc";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, endPoint, parameters));
		}

		[Fact]
		public void True_FooBarIdGuid_FooBarGuid()
		{
			const string pattern = "/foo/bar/{id:guid}";
			const string endPoint = "/foo/bar/D01E3FC6-2A16-43A0-AABB-CC9B67A494FD";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "D01E3FC6-2A16-43A0-AABB-CC9B67A494FD");
		}

		[Fact]
		public void False_FooBarIdGuid_FooBarNonGuid()
		{
			const string pattern = "/foo/bar/{id:guid}";
			const string endPoint = "/foo/bar/D01E3FC6-2A16-43A0-AABB-CC9B67A494FD-ERROR";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, endPoint, parameters));
		}

		[Fact]
		public void True_FooBarIdFloat_FooBar1()
		{
			const string pattern = "/foo/bar/{id:float}";
			const string endPoint = "/foo/bar/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}

		[Fact]
		public void True_FooBarIdFloat_FooBar1dot10()
		{
			const string pattern = "/foo/bar/{id:float}";
			const string endPoint = "/foo/bar/1.10";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1.10");
		}

		[Fact]
		public void False_FooBarIdFloat_FooBarAbc()
		{
			const string pattern = "/foo/bar/{id:float}";
			const string endPoint = "/foo/bar/abc";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, endPoint, parameters));
		}

		[Fact]
		public void True_FooBarDtDateTime_FooBarDateTime()
		{
			const string pattern = "/foo/bar/{dt:datetime}";
			const string endPoint = "/foo/bar/2018-01-04T05:52:20.698";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["dt"] != null);
			Assert.True(parameters["dt"] == "2018-01-04T05:52:20.698");
		}

		[Fact]
		public void True_FooBarBBool_FooBarTrue()
		{
			const string pattern = "/foo/bar/{b:bool}";
			const string endPoint = "/foo/bar/true";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["b"] != null);
			Assert.True(parameters["b"] == "true");
		}

		[Fact]
		public void True_FooBarBBool_FooBarFalse()
		{
			const string pattern = "/foo/bar/{b:bool}";
			const string endPoint = "/foo/bar/false";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteEndPointParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, endPoint, parameters));
			Assert.True(parameters["b"] != null);
			Assert.True(parameters["b"] == "false");
		}
	}
}
