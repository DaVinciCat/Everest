using System.Collections.Specialized;
using Everest.Routing;
using Xunit;

namespace Everest.Tests
{
	public class RoutingTests
	{
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
			const string pattern = "/foo/bar/:id";
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
		public void True_StarFoo_BazBarFoo()
		{
			const string pattern = "/a*/foo";
			const string url = "/baz/bar/foo";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, url, parameters));
			Assert.True(parameters["a"] != null);
			Assert.True(parameters["a"] == "baz/bar");
		}

		[Fact]
		public void True_StarFooId_BazBarFoo1()
		{
			const string pattern = "/a*/foo/:id";
			const string url = "/baz/bar/foo/1";

			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var segment = builder.Build(pattern);
			var parameters = new NameValueCollection();

			Assert.True(parser.TryParse(segment, url, parameters));
			Assert.True(parameters["a"] != null);
			Assert.True(parameters["a"] == "baz/bar");

			Assert.True(parameters["id"] != null);
			Assert.True(parameters["id"] == "1");
		}
	}
}
