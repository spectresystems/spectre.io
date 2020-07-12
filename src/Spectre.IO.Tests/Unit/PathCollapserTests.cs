using System;
using Shouldly;
using Spectre.IO.Internal;
using Spectre.IO.Testing.Xunit;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO
{
    public sealed class PathCollapserTests
    {
        public sealed class TheCollapseMethod
        {
            [Fact]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => PathCollapser.Collapse(null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("path");
            }

            [Fact]
            public void Should_Collapse_Relative_Path()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("hello/temp/test/../../world"));

                // Then
                path.ShouldBe("hello/world");
            }

            [Fact]
            public void Should_Collapse_Path_With_Separated_Ellipsis()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("hello/temp/../temp2/../world"));

                // Then
                path.ShouldBe("hello/world");
            }

            [WindowsFact]
            public void Should_Collapse_Path_With_Windows_Root()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("c:/hello/temp/test/../../world"));

                // Then
                path.ShouldBe("c:/hello/world");
            }

            [Fact]
            public void Should_Collapse_Path_With_Non_Windows_Root()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("/hello/temp/test/../../world"));

                // Then
                path.ShouldBe("/hello/world");
            }

            [WindowsFact]
            public void Should_Stop_Collapsing_When_Windows_Root_Is_Reached()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("c:/../../../../../../temp"));

                // Then
                path.ShouldBe("c:/temp");
            }

            [Fact]
            public void Should_Stop_Collapsing_When_Root_Is_Reached()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("/hello/../../../../../../temp"));

                // Then
                path.ShouldBe("/temp");
            }

            [Theory]
            [InlineData(".")]
            [InlineData("./")]
            [InlineData("/.")]
            public void Should_Collapse_Single_Dot_To_Single_Dot(string uncollapsedPath)
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath(uncollapsedPath));

                // Then
                path.ShouldBe(".");
            }

            [Fact]
            public void Should_Collapse_Single_Dot_With_Ellipsis()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("./.."));

                // Then
                path.ShouldBe(".");
            }

            [Theory]
            [InlineData("./a", "a")]
            [InlineData("a/./b", "a/b")]
            [InlineData("/a/./b", "/a/b")]
            [InlineData("a/b/.", "a/b")]
            [InlineData("/a/b/.", "/a/b")]
            [InlineData("/./a/b", "/a/b")]
            public void Should_Collapse_Single_Dot(string uncollapsedPath, string collapsedPath)
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath(uncollapsedPath));

                // Then
                path.ShouldBe(collapsedPath);
            }
        }
    }
}