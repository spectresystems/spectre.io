using Shouldly;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO
{
    public sealed class PathTests
    {
        private sealed class TestingPath : Path
        {
            public TestingPath(string path)
                : base(path)
            {
            }
        }

        public sealed class TheConstructor
        {
            [Fact]
            public void Current_Directory_Returns_Empty_Path()
            {
                // Given, When
                var path = new TestingPath("./");

                // Then
                path.FullPath.ShouldBeEmpty();
            }

            [Fact]
            public void Will_Normalize_Path_Separators()
            {
                // Given, When
                var path = new TestingPath("shaders\\basic");

                // Then
                path.FullPath.ShouldBe("shaders/basic");
            }

            [Fact]
            public void Will_Normalize_UNC_Path_Separators()
            {
                // Given, When
                var path = new TestingPath(@"\\foo/bar\qux");

                // Then
                path.FullPath.ShouldBe(@"\\foo\bar\qux");
            }

            [Theory]
            [InlineData(" foo/bar ", "foo/bar")]
            [InlineData(@" \\foo\bar ", @"\\foo\bar")]
            public void Will_Trim_WhiteSpace_From_Path(string input, string expected)
            {
                // Given, When
                var path = new TestingPath(input);

                // Then
                path.FullPath.ShouldBe(expected);
            }

            [Theory]
            [InlineData("foo bar/qux")]
            [InlineData(@"\\foo bar\qux")]
            public void Will_Not_Remove_WhiteSpace_Within_Path(string expected)
            {
                // Given, When
                var path = new TestingPath(expected);

                // Then
                path.FullPath.ShouldBe(expected);
            }

            [Theory]
            [InlineData("/Hello/World/", "/Hello/World")]
            [InlineData("\\Hello\\World\\", "/Hello/World")]
            [InlineData("file.txt/", "file.txt")]
            [InlineData("file.txt\\", "file.txt")]
            [InlineData("Temp/file.txt/", "Temp/file.txt")]
            [InlineData("Temp\\file.txt\\", "Temp/file.txt")]
            [InlineData(@"\\foo\bar\", @"\\foo\bar")]
            [InlineData(@"\\foo\bar/", @"\\foo\bar")]
            public void Should_Remove_Trailing_Slashes(string value, string expected)
            {
                // Given, When
                var path = new TestingPath(value);

                // Then
                path.FullPath.ShouldBe(expected);
            }

            [Fact]
            public void Should_Not_Remove_Trailing_Slash_For_Root()
            {
                // Given, When
                var path = new TestingPath("/");

                // Then
                path.FullPath.ShouldBe("/");
            }

            [Fact]
            public void Should_Not_Remove_Trailing_Slash_For_UNC_Root()
            {
                // Given, When
                var path = new TestingPath(@"\\");

                // Then
                path.FullPath.ShouldBe(@"\\");
            }
        }

        public sealed class TheIsUNCProperty
        {
            [Theory]
            [InlineData(@"\\Hello\World", true)]
            [InlineData("Hello/World", false)]
            [InlineData("./Hello/World", false)]
            public void Should_Return_Whether_Or_Not_A_Path_Is_An_UNC_Path(string pathName, bool expected)
            {
                // Given
                var path = new TestingPath(pathName);

                // When
                var result = path.IsUNC;

                // Then
                result.ShouldBe(expected);
            }
        }

        public sealed class TheSegmentsProperty
        {
            [Theory]
            [InlineData("Hello/World")]
            [InlineData("./Hello/World/")]
            public void Should_Return_Segments_Of_Path(string pathName)
            {
                // Given
                var path = new TestingPath(pathName);

                // When, Then
                path.Segments.Count.ShouldBe(2);
                path.Segments[0].ShouldBe("Hello");
                path.Segments[1].ShouldBe("World");
            }

            [Theory]
            [InlineData(@"\\Hello\World")]
            public void Should_Return_Segments_Of_UNC_Path(string pathName)
            {
                // Given
                var path = new TestingPath(pathName);

                // When, Then
                path.Segments.Count.ShouldBe(3);
                path.Segments[0].ShouldBe(@"\\");
                path.Segments[1].ShouldBe("Hello");
                path.Segments[2].ShouldBe("World");
            }

            [Theory]
            [InlineData("/Hello/World")]
            [InlineData("/Hello/World/")]
            public void Should_Return_Segments_Of_Path_And_Leave_Absolute_Directory_Separator_Intact(string pathName)
            {
                // Given
                var path = new TestingPath(pathName);

                // When, Then
                path.Segments.Count.ShouldBe(2);
                path.Segments[0].ShouldBe("/Hello");
                path.Segments[1].ShouldBe("World");
            }
        }

        public sealed class TheFullPathProperty
        {
            [Fact]
            public void Should_Return_Full_Path()
            {
                // Given, When
                var path = new TestingPath("shaders/basic");

                // Then
                path.FullPath.ShouldBe("shaders/basic");
            }
        }

        public sealed class TheIsRelativeProperty
        {
            [Theory]
            [InlineData("assets/shaders", true)]
            [InlineData("assets/shaders/basic.frag", true)]
            [InlineData("/assets/shaders", false)]
            [InlineData("/assets/shaders/basic.frag", false)]
            public void Should_Return_Whether_Or_Not_A_Path_Is_Relative(string fullPath, bool expected)
            {
                // Given, When
                var path = new TestingPath(fullPath);

                // Then
                path.IsRelative.ShouldBe(expected);
            }

            [Fact]
            public void An_UNC_Path_Is_Always_Considered_To_Be_Absolute()
            {
                // Given, When
                var path = new TestingPath(@"\\foo\bar");

                // Then
                path.IsRelative.ShouldBeFalse();
            }

            [Theory]
            [InlineData("c:/assets/shaders", false)]
            [InlineData("c:/assets/shaders/basic.frag", false)]
            [InlineData("c:/", false)]
            [InlineData("c:", false)]
            public void Should_Return_Whether_Or_Not_A_Path_Is_Relative_On_Windows(string fullPath, bool expected)
            {
                // Given, When
                var path = new TestingPath(fullPath);

                // Then
                path.IsRelative.ShouldBe(expected);
            }
        }

        public sealed class TheToStringMethod
        {
            [Theory]
            [InlineData("foo/bar")]
            [InlineData(@"\\foo\bar")]
            [InlineData(@"\\foo")]
            public void Should_Return_The_Full_Path(string expected)
            {
                // Given, When
                var path = new TestingPath(expected);

                // Then
                path.ToString().ShouldBe(expected);
            }
        }
    }
}