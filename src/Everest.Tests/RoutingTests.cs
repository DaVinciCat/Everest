using NUnit.Framework;

namespace Everest.Tests
{
	public class RoutingTests
	{
		[Test]
        public void Test()
        {
            Assert.True(true);
        }

	//	[Fact]
	//	public async Task False_FooBar_Slash()
	//	{
	//		const string pattern = "/foo/bar";
	//		const string endPoint = "/";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(await parser.TryParseAsync(segment, endPoint));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar__Foo_Bar()
	//	{
	//		const string pattern = "/foo/bar";
	//		const string endPoint = "/foo/bar";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint));
	//	}

	//	[Fact]
	//	public void False_Foo_Bar__Bar()
	//	{
	//		const string pattern = "/foo/bar";
	//		const string endPoint = "/bar";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void False_Foo_Bar__Foo_Baz()
	//	{
	//		const string pattern = "/foo/bar";
	//		const string endPoint = "/foo/baz";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_IdString__Foo_Bar_1()
	//	{
	//		const string pattern = "/foo/bar/{id:string}";
	//		const string endPoint = "/foo/bar/1";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id"] != null);
	//		Assert.True(parameters["id"] == "1");
	//	}

	//	[Fact]
	//	public void True_Foo_Id1String_Bar_Id2String__Foo_1_Bar_2()
	//	{
	//		const string pattern = "/foo/{id1:string}/bar/{id2:string}";
	//		const string endPoint = "/foo/1/bar/2";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id1"] != null);
	//		Assert.True(parameters["id1"] == "1");

	//		Assert.True(parameters["id2"] != null);
	//		Assert.True(parameters["id2"] == "2");
	//	}

	//	[Fact]
	//	public void False_Foo_Id1String_Bar_Id2String__Foo_1()
	//	{
	//		const string pattern = "/foo/{id1:string}/bar/{id2:string}";
	//		const string endPoint = "/foo/1";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void False_Foo_Bar__Foo_Bar_QueryId1()
	//	{
	//		const string pattern = "/foo/bar";
	//		const string endPoint = "/foo/bar?id=1";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_IdInt__Foo_Bar_1()
	//	{
	//		const string pattern = "/foo/bar/{id:int}";
	//		const string endPoint = "/foo/bar/1";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id"] != null);
	//		Assert.True(parameters["id"] == "1");
	//	}

	//	[Fact]
	//	public void False_Foo_Bar_IdInt__Foo_Bar_1dot10()
	//	{
	//		const string pattern = "/foo/bar/{id:int}";
	//		const string endPoint = "/foo/bar/1.10";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void False_Foo_Bar_IdInt__Foo_Bar_Abc()
	//	{
	//		const string pattern = "/foo/bar/{id:int}";
	//		const string endPoint = "/foo/bar/abc";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_IdGuid__Foo_Bar_Guid()
	//	{
	//		const string pattern = "/foo/bar/{id:guid}";
	//		const string endPoint = "/foo/bar/D01E3FC6-2A16-43A0-AABB-CC9B67A494FD";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id"] != null);
	//		Assert.True(parameters["id"] == "D01E3FC6-2A16-43A0-AABB-CC9B67A494FD");
	//	}

	//	[Fact]
	//	public void False_Foo_Bar_IdGuid__Foo_Bar_NonGuid()
	//	{
	//		const string pattern = "/foo/bar/{id:guid}";
	//		const string endPoint = "/foo/bar/D01E3FC6-2A16-43A0-AABB-CC9B67A494FD-ERROR";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_IdFloat__Foo_Bar_1()
	//	{
	//		const string pattern = "/foo/bar/{id:float}";
	//		const string endPoint = "/foo/bar/1";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id"] != null);
	//		Assert.True(parameters["id"] == "1");
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_IdFloat__Foo_Bar_1dot10()
	//	{
	//		const string pattern = "/foo/bar/{id:float}";
	//		const string endPoint = "/foo/bar/1.10";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["id"] != null);
	//		Assert.True(parameters["id"] == "1.10");
	//	}

	//	[Fact]
	//	public void False_Foo_Bar_IdFloat__Foo_Bar_Abc()
	//	{
	//		const string pattern = "/foo/bar/{id:float}";
	//		const string endPoint = "/foo/bar/abc";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.False(parser.TryParseAsync(segment, endPoint, out _));
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_DtDateTime__Foo_Bar_DateTime()
	//	{
	//		const string pattern = "/foo/bar/{dt:datetime}";
	//		const string endPoint = "/foo/bar/2018-01-04T05:52:20.698";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["dt"] != null);
	//		Assert.True(parameters["dt"] == "2018-01-04T05:52:20.698");
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_BBool__Foo_Bar_True()
	//	{
	//		const string pattern = "/foo/bar/{b:bool}";
	//		const string endPoint = "/foo/bar/true";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["b"] != null);
	//		Assert.True(parameters["b"] == "true");
	//	}

	//	[Fact]
	//	public void True_Foo_Bar_BBool__Foo_Bar_False()
	//	{
	//		const string pattern = "/foo/bar/{b:bool}";
	//		const string endPoint = "/foo/bar/false";

	//		var builder = new RouteSegmentBuilder();
	//		var parser = new RouteSegmentParser();
	//		var segment = builder.Build(pattern);

	//		Assert.True(parser.TryParseAsync(segment, endPoint, out var parameters));
	//		Assert.True(parameters["b"] != null);
	//		Assert.True(parameters["b"] == "false");
	//	}
	}
}
