using NSubstitute;
using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO;

public sealed class DirectoryPathTests
{
    public sealed class TheGetDirectoryNameMethod
    {
        [Theory]
        [InlineData("C:/Data", "Data")]
        [InlineData("C:/Data/Work", "Work")]
        [InlineData("C:/Data/Work/file.txt", "file.txt")]
        [InlineData(@"\\Data\Work", "Work")]
        public void Should_Return_Directory_Name(string directoryPath, string name)
        {
            // Given
            var path = new DirectoryPath(directoryPath);

            // When
            var result = path.GetDirectoryName();

            // Then
            result.ShouldBe(name);
        }
    }

    public sealed class TheGetParentMethod
    {
        [Theory]
        [InlineData("C:/Data", "C:/")]
        [InlineData("C:/Data/Work", "C:/Data")]
        [InlineData("C:/Data/Work/Other", "C:/Data/Work")]
        [InlineData(@"\\Data\Work", @"\\Data")]
        [InlineData(@"\\Data\Work\Other", @"\\Data\Work")]
        public void Should_Return_Parent(string directoryPath, string parent)
        {
            // Given
            var path = new DirectoryPath(directoryPath);

            // When
            var result = path.GetParent();

            // Then
            result.FullPath.ShouldBe(parent);
        }

        [Theory]
        [InlineData(@"\\")]
        [InlineData(@"\\Data")]
        [InlineData("C:/")]
        [InlineData("C:")]
        public void Should_Return_Null_For_Root_Paths(string directoryPath)
        {
            // Given
            var path = new DirectoryPath(directoryPath);

            // When
            var result = path.GetParent();

            // Then
            result.ShouldBeNull();
        }
    }

    public sealed class TheGetFilePathMethod
    {
        [Fact]
        public void Should_Throw_If_Path_Is_Null()
        {
            // Given
            var path = new DirectoryPath("assets");

            // When
            var result = Record.Exception(() => path.GetFilePath(null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("path");
        }

        [Theory]
        [InlineData("c:/assets/shaders/", "simple.frag", "c:/assets/shaders/simple.frag")]
        [InlineData("c:/", "simple.frag", "c:/simple.frag")]
        [InlineData("c:/assets/shaders/", "test/simple.frag", "c:/assets/shaders/simple.frag")]
        [InlineData("c:/", "test/simple.frag", "c:/simple.frag")]
        [InlineData("assets/shaders", "simple.frag", "assets/shaders/simple.frag")]
        [InlineData("assets/shaders/", "simple.frag", "assets/shaders/simple.frag")]
        [InlineData("/assets/shaders/", "simple.frag", "/assets/shaders/simple.frag")]
        [InlineData("assets/shaders", "test/simple.frag", "assets/shaders/simple.frag")]
        [InlineData("assets/shaders/", "test/simple.frag", "assets/shaders/simple.frag")]
        [InlineData("/assets/shaders/", "test/simple.frag", "/assets/shaders/simple.frag")]
        [InlineData(@"\\foo\bar", "qux.txt", @"\\foo\bar\qux.txt")]
        [InlineData(@"\\foo\bar", "baz/qux.txt", @"\\foo\bar\qux.txt")]
        public void Should_Combine_Paths(string first, string second, string expected)
        {
            // Given
            var path = new DirectoryPath(first);

            // When
            var result = path.GetFilePath(new FilePath(second));

            // Then
            result.FullPath.ShouldBe(expected);
        }
    }

    public sealed class TheCombineWithFilePathMethod
    {
        [Fact]
        public void Should_Throw_If_Path_Is_Null()
        {
            // Given
            var path = new DirectoryPath("assets");

            // When
            var result = Record.Exception(() => path.CombineWithFilePath(null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("path");
        }

        [Theory]
        [InlineData("assets/shaders", "simple.frag", "assets/shaders/simple.frag")]
        [InlineData("assets/shaders/", "simple.frag", "assets/shaders/simple.frag")]
        [InlineData("/assets/shaders/", "simple.frag", "/assets/shaders/simple.frag")]
        [InlineData("assets/shaders", "test/simple.frag", "assets/shaders/test/simple.frag")]
        [InlineData("assets/shaders/", "test/simple.frag", "assets/shaders/test/simple.frag")]
        [InlineData("/assets/shaders/", "test/simple.frag", "/assets/shaders/test/simple.frag")]
        [InlineData("/", "test/simple.frag", "/test/simple.frag")]
        public void Should_Combine_Paths(string first, string second, string expected)
        {
            // Given
            var path = new DirectoryPath(first);

            // When
            var result = path.CombineWithFilePath(new FilePath(second));

            // Then
            result.FullPath.ShouldBe(expected);
        }

        [Theory]
        [InlineData("c:/assets/shaders/", "simple.frag", "c:/assets/shaders/simple.frag")]
        [InlineData("c:/", "simple.frag", "c:/simple.frag")]
        [InlineData("c:/assets/shaders/", "test/simple.frag", "c:/assets/shaders/test/simple.frag")]
        [InlineData("c:/", "test/simple.frag", "c:/test/simple.frag")]
        [InlineData(@"\\", "qux.txt", @"\\qux.txt")]
        [InlineData(@"\\foo\bar", "qux.txt", @"\\foo\bar\qux.txt")]
        [InlineData(@"\\", "baz/qux.txt", @"\\baz\qux.txt")]
        [InlineData(@"\\foo\bar", "baz/qux.txt", @"\\foo\bar\baz\qux.txt")]
        public void Should_Combine_Windows_Paths(string first, string second, string expected)
        {
            // Given
            var path = new DirectoryPath(first);

            // When
            var result = path.CombineWithFilePath(new FilePath(second));

            // Then
            result.FullPath.ShouldBe(expected);
        }

        [Fact]
        public void Can_Not_Combine_Directory_Path_With_Absolute_File_Path()
        {
            // Given
            var path = new DirectoryPath("foo");

            // When
            var result = Record.Exception(() => path.CombineWithFilePath(new FilePath("/other/asset.txt")));

            // Then
            result.ShouldBeOfType<InvalidOperationException>()
                .And().Message.ShouldBe("Cannot combine a directory path with an absolute file path.");
        }

        [Fact]
        public void Can_Not_Combine_Directory_Path_With_Absolute_UNC_File_Path()
        {
            // Given
            var path = new DirectoryPath(@"\\foo");

            // When
            var result = Record.Exception(() => path.CombineWithFilePath(new FilePath("/other/asset.txt")));

            // Then
            result.ShouldBeOfType<InvalidOperationException>()
                .And().Message.ShouldBe("Cannot combine a directory path with an absolute file path.");
        }
    }

    public sealed class TheCombineMethod
    {
        [Theory]
        [InlineData("assets/shaders", "simple", "assets/shaders/simple")]
        [InlineData("assets/shaders/", "simple", "assets/shaders/simple")]
        [InlineData("/assets/shaders/", "simple", "/assets/shaders/simple")]
        public void Should_Combine_Paths(string first, string second, string expected)
        {
            // Given
            var path = new DirectoryPath(first);

            // When
            var result = path.Combine(new DirectoryPath(second));

            // Then
            result.FullPath.ShouldBe(expected);
        }

        [Theory]
        [InlineData("c:/assets/shaders/", "simple", "c:/assets/shaders/simple")]
        [InlineData("c:/", "simple", "c:/simple")]
        [InlineData(@"\\", "foo", @"\\foo")]
        [InlineData(@"\\", "foo/", @"\\foo")]
        [InlineData(@"\\", "foo\\", @"\\foo")]
        [InlineData(@"\\foo", "bar", @"\\foo\bar")]
        [InlineData(@"\\foo", "bar/", @"\\foo\bar")]
        [InlineData(@"\\foo", "bar\\", @"\\foo\bar")]
        public void Should_Combine_Windows_Paths(string first, string second, string expected)
        {
            // Given
            var path = new DirectoryPath(first);

            // When
            var result = path.Combine(new DirectoryPath(second));

            // Then
            result.FullPath.ShouldBe(expected);
        }

        [Fact]
        public void Should_Throw_If_Path_Is_Null()
        {
            // Given
            var path = new DirectoryPath("assets");

            // When
            var result = Record.Exception(() => path.Combine(null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("path");
        }

        [Fact]
        public void Can_Not_Combine_Directory_Path_With_Absolute_Directory_Path()
        {
            // Given
            var path = new DirectoryPath("assets");

            // When
            var result = Record.Exception(() => path.Combine(new DirectoryPath("/other/assets")));

            // Then
            result.ShouldBeOfType<InvalidOperationException>()
                .And().Message.ShouldBe("Cannot combine a directory path with an absolute directory path.");
        }

        [Theory]
        [InlineData("C:/foo/bar")]
        [InlineData(@"\\foo\bar")]
        public void Can_Not_Combine_Directory_Path_With_Absolute_Windows_Directory_Path(string input)
        {
            // Given
            var path = new DirectoryPath("assets");

            // When
            var result = Record.Exception(() => path.Combine(new DirectoryPath(input)));

            // Then
            result.ShouldBeOfType<InvalidOperationException>()
                .And().Message.ShouldBe("Cannot combine a directory path with an absolute directory path.");
        }
    }

    public sealed class TheMakeAbsoluteMethod
    {
        public sealed class ThatTakesAnEnvironment
        {
            [Fact]
            public void Should_Throw_If_Provided_Environment_Is_Null()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Record.Exception(
                    () => path.MakeAbsolute((IEnvironment)null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("environment");
            }

            [Fact]
            public void Should_Create_New_Absolute_Path_When_Path_Is_Relative()
            {
                // Given
                var environment = Substitute.For<IEnvironment>();
                environment.WorkingDirectory.Returns("/Working");
                var path = new DirectoryPath("assets");

                // When
                var result = path.MakeAbsolute(environment);

                // Then
                result.FullPath.ShouldBe("/Working/assets");
            }

            [Fact]
            public void Should_Create_New_Absolute_Path_Identical_To_The_Path()
            {
                // Given
                var environment = Substitute.For<IEnvironment>();
                var path = new DirectoryPath("/assets");

                // When
                var result = path.MakeAbsolute(environment);

                // Then
                result.FullPath.ShouldBe("/assets");
            }

            [Theory]
            [InlineData("C:/foo")]
            [InlineData(@"\\foo")]
            public void Should_Create_New_Absolute_Windows_Path_Identical_To_The_Path(string fullPath)
            {
                // Given
                var environment = Substitute.For<IEnvironment>();
                var path = new DirectoryPath(fullPath);

                // When
                var result = path.MakeAbsolute(environment);

                // Then
                result.FullPath.ShouldBe(fullPath);
                result.ShouldNotBeSameAs(path);
            }

            [Fact]
            public void Should_Expand_Home_Directory()
            {
                // Given
                var path = new DirectoryPath("~/test");
                var environment = new FakeEnvironment(PlatformFamily.Linux);

                // When
                var result = path.MakeAbsolute(environment);

                // Then
                result.FullPath.ShouldBe("/home/JohnDoe/test");
            }
        }

        public sealed class ThatTakesAnotherDirectoryPath
        {
            [Fact]
            public void Should_Throw_If_Provided_Path_Is_Null()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Record.Exception(
                    () => path.MakeAbsolute((DirectoryPath)null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("path");
            }

            [Fact]
            public void Should_Throw_If_Provided_Path_Is_Relative()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Record.Exception(() => path.MakeAbsolute("Working"));

                // Then
                result.ShouldBeOfType<IOException>()
                    .And().Message.ShouldBe("The provided path cannot be relative.");
            }

            [Fact]
            public void Should_Create_New_Absolute_Path_When_Path_Is_Relative()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = path.MakeAbsolute("/absolute");

                // Then
                result.FullPath.ShouldBe("/absolute/assets");
            }

            [Fact]
            public void Should_Create_New_Absolute_Path_Identical_To_The_Path()
            {
                // Given
                var path = new DirectoryPath("/assets");

                // When
                var result = path.MakeAbsolute("/absolute");

                // Then
                result.FullPath.ShouldBe("/assets");
            }
        }
    }

    public sealed class TheCollapseMethod
    {
        [Fact]
        public void Should_Collapse_Relative_Path()
        {
            // Given, When
            var path = new DirectoryPath("hello/temp/test/../../world").Collapse();

            // Then
            path.FullPath.ShouldBe("hello/world");
        }

        [Fact]
        public void Should_Collapse_Path_With_Separated_Ellipsis()
        {
            // Given, When
            var path = new DirectoryPath("hello/temp/../temp2/../world").Collapse();

            // Then
            path.FullPath.ShouldBe("hello/world");
        }

        [Fact]
        public void Should_Collapse_Path_With_Windows_Root()
        {
            // Given, When
            var path = new DirectoryPath("c:/hello/temp/test/../../world").Collapse();

            // Then
            path.FullPath.ShouldBe("c:/hello/world");
        }

        [Fact]
        public void Should_Collapse_Path_With_Non_Windows_Root()
        {
            // Given, When
            var path = new DirectoryPath("/hello/temp/test/../../world").Collapse();

            // Then
            path.FullPath.ShouldBe("/hello/world");
        }

        [Fact]
        public void Should_Stop_Collapsing_When_Windows_Root_Is_Reached()
        {
            // Given, When
            var path = new DirectoryPath("c:/../../../../../../temp").Collapse();

            // Then
            path.FullPath.ShouldBe("c:/temp");
        }

        [Fact]
        public void Should_Stop_Collapsing_When_Root_Is_Reached()
        {
            // Given, When
            var path = new DirectoryPath("/hello/../../../../../../temp").Collapse();

            // Then
            path.FullPath.ShouldBe("/temp");
        }

        [Theory]
        [InlineData(".")]
        [InlineData("./")]
        [InlineData("/.")]
        public void Should_Collapse_Single_Dot_To_Single_Dot(string uncollapsedPath)
        {
            // Given, When
            var path = new DirectoryPath(uncollapsedPath).Collapse();

            // Then
            path.FullPath.ShouldBe(".");
        }

        [Fact]
        public void Should_Collapse_Single_Dot_With_Ellipsis()
        {
            // Given, When
            var path = new DirectoryPath("./..").Collapse();

            // Then
            path.FullPath.ShouldBe(".");
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
            var path = new DirectoryPath(uncollapsedPath).Collapse();

            // Then
            path.FullPath.ShouldBe(collapsedPath);
        }
    }

    public sealed class TheExpandMethod
    {
        [Theory]
        [InlineData(PlatformFamily.Windows, "", "")]
        [InlineData(PlatformFamily.Windows, "~", "C:/Users/JohnDoe")]
        [InlineData(PlatformFamily.Windows, "~/", "C:/Users/JohnDoe")]
        [InlineData(PlatformFamily.Windows, "~/lol", "C:/Users/JohnDoe/lol")]
        [InlineData(PlatformFamily.Linux, "", "")]
        [InlineData(PlatformFamily.Linux, "~", "/home/JohnDoe")]
        [InlineData(PlatformFamily.Linux, "~/", "/home/JohnDoe")]
        [InlineData(PlatformFamily.Linux, "~/lol", "/home/JohnDoe/lol")]
        public void Should_Expand_Home_Directory_If_First_In_Path(PlatformFamily family, string input, string expected)
        {
            // Given
            var environment = new FakeEnvironment(family);
            var path = new DirectoryPath(input);

            // When
            var result = path.Expand(environment);

            // Then
            result.FullPath.ShouldBe(expected);
        }
    }

    public sealed class TheGetRelativePathMethod
    {
        public sealed class WithDirectoryPath
        {
            public sealed class InWindowsFormat
            {
                [Theory]
                [InlineData("C:/A/B/C", "C:/A/B/C", ".")]
                [InlineData("C:/", "C:/", ".")]
                [InlineData("C:/A/B/C", "C:/A/D/E", "../../D/E")]
                [InlineData("C:/A/B/C", "C:/", "../../..")]
                [InlineData("C:/A/B/C/D/E/F", "C:/A/B/C", "../../..")]
                [InlineData("C:/A/B/C", "C:/A/B/C/D/E/F", "D/E/F")]
                [InlineData(@"\\A\B\C", @"\\A\B\C", ".")]
                [InlineData(@"\\", @"\\", ".")]
                [InlineData(@"\\A\B\C", @"\\A\D\E", "../../D/E")]
                [InlineData(@"\\A\B\C", @"\\", "../../..")]
                [InlineData(@"\\A\B\C/D/E/F", @"\\A\B\C", "../../..")]
                [InlineData(@"\\A\B\C", @"\\A\B\C\D\E\F", "D/E/F")]
                public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var relativePath = path.GetRelativePath(new DirectoryPath(to));

                    // Then
                    relativePath.FullPath.ShouldBe(expected);
                }

                [Theory]
                [InlineData("C:/A/B/C", "D:/A/B/C")]
                [InlineData("C:/A/B", "D:/E/")]
                [InlineData("C:/", "B:/")]
                [InlineData(@"\\A\B\C", "D:/A/B/C")]
                [InlineData(@"\\A\B", "D:/E/")]
                [InlineData(@"\\", "B:/")]
                public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath(to)));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Paths must share a common prefix.");
                }

                [Fact]
                public void Should_Throw_If_Target_DirectoryPath_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("C:/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath((DirectoryPath)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("to");
                }

                [Fact]
                public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("A");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath("C:/D/E/F")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Source path must be an absolute path.");
                }

                [Theory]
                [InlineData("C:/A/B/C")]
                [InlineData(@"\\A\B\C")]
                public void Should_Throw_If_Target_DirectoryPath_Is_Relative(string input)
                {
                    // Given
                    var path = new DirectoryPath(input);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath("D")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Target path must be an absolute path.");
                }
            }

            public sealed class InUnixFormat
            {
                [Theory]
                [InlineData("/C/A/B/C", "/C/A/B/C", ".")]
                [InlineData("/C/", "/C/", ".")]
                [InlineData("/C/A/B/C", "/C/A/D/E", "../../D/E")]
                [InlineData("/C/A/B/C", "/C/", "../../..")]
                [InlineData("/C/A/B/C/D/E/F", "/C/A/B/C", "../../..")]
                [InlineData("/C/A/B/C", "/C/A/B/C/D/E/F", "D/E/F")]
                public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var relativePath = path.GetRelativePath(new DirectoryPath(to));

                    // Then
                    relativePath.FullPath.ShouldBe(expected);
                }

                [Theory]
                [InlineData("/C/A/B/C", "/D/A/B/C")]
                [InlineData("/C/A/B", "/D/E/")]
                [InlineData("/C/", "/B/")]
                public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath(to)));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Paths must share a common prefix.");
                }

                [Fact]
                public void Should_Throw_If_Target_DirectoryPath_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("/C/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath((DirectoryPath)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("to");
                }

                [Fact]
                public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("A");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath("/C/D/E/F")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Source path must be an absolute path.");
                }

                [Fact]
                public void Should_Throw_If_Target_DirectoryPath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("/C/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath("D")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Target path must be an absolute path.");
                }
            }
        }

        public sealed class WithFilePath
        {
            public sealed class InWindowsFormat
            {
                [Theory]
                [InlineData("C:/A/B/C", "C:/A/B/C/hello.txt", "hello.txt")]
                [InlineData("C:/", "C:/hello.txt", "hello.txt")]
                [InlineData("C:/A/B/C", "C:/A/D/E/hello.txt", "../../D/E/hello.txt")]
                [InlineData("C:/A/B/C", "C:/hello.txt", "../../../hello.txt")]
                [InlineData("C:/A/B/C/D/E/F", "C:/A/B/C/hello.txt", "../../../hello.txt")]
                [InlineData("C:/A/B/C", "C:/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                [InlineData(@"\\A\B\C", @"\\A\B\C\D\E\F\hello.txt", "D/E/F/hello.txt")]
                [InlineData(@"\\", @"\\hello.txt", "hello.txt")]
                [InlineData(@"\\A\B\C", @"\\A\D\E\hello.txt", "../../D/E/hello.txt")]
                [InlineData(@"\\A\B\C", @"\\hello.txt", "../../../hello.txt")]
                [InlineData(@"\\A\B\C\D\E\F", @"\\A\B\C\hello.txt", "../../../hello.txt")]
                public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var relativePath = path.GetRelativePath(new FilePath(to));

                    // Then
                    relativePath.FullPath.ShouldBe(expected);
                }

                [Theory]
                [InlineData("C:/A/B/C", "D:/A/B/C/hello.txt")]
                [InlineData("C:/A/B", "D:/E/hello.txt")]
                [InlineData("C:/", "B:/hello.txt")]
                [InlineData(@"\\A\B\C", "D:/A/B/C/hello.txt")]
                [InlineData(@"\\A\B", "D:/E/hello.txt")]
                [InlineData(@"\\", "B:/hello.txt")]
                public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath(to)));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Paths must share a common prefix.");
                }

                [Fact]
                public void Should_Throw_If_Target_FilePath_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("C:/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath((FilePath)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("to");
                }

                [Fact]
                public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("A");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath("C:/D/E/F/hello.txt")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Source path must be an absolute path.");
                }

                [Theory]
                [InlineData("C:/A/B/C")]
                [InlineData(@"\\A\B\C")]
                public void Should_Throw_If_Target_FilePath_Is_Relative(string input)
                {
                    // Given
                    var path = new DirectoryPath(input);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath("D/hello.txt")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Target path must be an absolute path.");
                }
            }

            public sealed class InUnixFormat
            {
                [Theory]
                [InlineData("/C/A/B/C", "/C/A/B/C/hello.txt", "hello.txt")]
                [InlineData("/C/", "/C/hello.txt", "hello.txt")]
                [InlineData("/C/A/B/C", "/C/A/D/E/hello.txt", "../../D/E/hello.txt")]
                [InlineData("/C/A/B/C", "/C/hello.txt", "../../../hello.txt")]
                [InlineData("/C/A/B/C/D/E/F", "/C/A/B/C/hello.txt", "../../../hello.txt")]
                [InlineData("/C/A/B/C", "/C/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var relativePath = path.GetRelativePath(new FilePath(to));

                    // Then
                    relativePath.FullPath.ShouldBe(expected);
                }

                [Theory]
                [InlineData("/C/A/B/C", "/D/A/B/C/hello.txt")]
                [InlineData("/C/A/B", "/D/E/hello.txt")]
                [InlineData("/C/", "/B/hello.txt")]
                public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                {
                    // Given
                    var path = new DirectoryPath(from);

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath(to)));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Paths must share a common prefix.");
                }

                [Fact]
                public void Should_Throw_If_Target_FilePath_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("/C/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath((FilePath)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("to");
                }

                [Fact]
                public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("A");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath("/C/D/E/F/hello.txt")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Source path must be an absolute path.");
                }

                [Fact]
                public void Should_Throw_If_Target_FilePath_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("/C/A/B/C");

                    // When
                    var result = Record.Exception(() => path.GetRelativePath(new FilePath("D/hello.txt")));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Target path must be an absolute path.");
                }
            }
        }
    }
}