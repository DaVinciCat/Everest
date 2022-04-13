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
			const string url = "/";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, url, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBar_FooBar()
		{
			const string pattern = "/foo/bar";
			const string url = "/foo/bar";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);

			Assert.True(parser.TryParse(segment, url, new NameValueCollection()));
		}

		[Fact]
		public void False_FooBar_Bar()
		{
			const string pattern = "/foo/bar";
			const string url = "/bar";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, url, new NameValueCollection()));
		}

		[Fact]
		public void False_FooBar_FooBaz()
		{
			const string pattern = "/foo/bar";
			const string url = "/foo/baz";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);

			Assert.False(parser.TryParse(segment, url, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBarId_FooBar1()
		{
			const string pattern = "/foo/bar/{id}";
			const string url = "/foo/bar/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, url, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}

		[Fact]
		public void True_FooId1BarId2_Foo1Bar2()
		{
			const string pattern = "/foo/{id1}/bar/{id2}";
			const string url = "/foo/1/bar/2";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, url, parameters));
			Assert.True(parameters["id1"] != null);
			Assert.True(parameters["id1"] == "1");

			Assert.True(parameters["id2"] != null);
			Assert.True(parameters["id2"] == "2");
		}

		[Fact]
		public void False_FooId1BarId2_Foo1()
		{
			const string pattern = "/foo/{id1}/bar/{id2}";
			const string url = "/foo/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, url, parameters));
		}

		[Fact]
		public void True_FooBar_FooBarQueryId()
		{
			const string pattern = "/foo/bar";
			const string url = "/foo/bar?id=1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);

			Assert.True(parser.TryParse(segment, url, new NameValueCollection()));
		}

		[Fact]
		public void True_FooBarIdInt_FooBar1()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string url = "/foo/bar/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, url, parameters));
			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}

		[Fact]
		public void False_FooBarIdInt_FooBar1dot10()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string url = "/foo/bar/1.10";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, url, parameters));
		}

		[Fact]
		public void False_FooBarIdInt_FooBarAbc()
		{
			const string pattern = "/foo/bar/{id:int}";
			const string url = "/foo/bar/abc";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.False(parser.TryParse(segment, url, parameters));
		}
	}
}
