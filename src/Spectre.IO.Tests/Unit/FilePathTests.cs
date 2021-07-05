using System;
using NSubstitute;
using Shouldly;
using Spectre.IO.Testing;
using Spectre.IO.Testing.Xunit;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO
{
    public sealed class FilePathTests
    {
        public sealed class TheHasExtensionProperty
        {
            [Theory]
            [InlineData("assets/shaders/basic.txt", true)]
            [InlineData("assets/shaders/basic", false)]
            [InlineData("assets/shad.ers/basic", false)]
            [InlineData("assets/shaders/basic/", false)]
            [InlineData("assets/shad.ers/basic/", false)]
            public void Can_See_If_A_Path_Has_An_Extension(string fullPath, bool expected)
            {
                // Given, When
                var path = new FilePath(fullPath);

                // Then
                path.HasExtension.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/foo/bar/baz.txt", true)]
            [InlineData("C:/foo/bar/baz", false)]
            [InlineData("C:/foo/bar.baz/qux", false)]
            [InlineData("C:/foo/bar/baz/", false)]
            [InlineData(@"\\foo\bar\baz.txt", true)]
            [InlineData(@"\\foo\bar\baz", false)]
            [InlineData(@"\\foo\bar.baz\qux", false)]
            [InlineData(@"\\foo\bar\baz\", false)]
            public void Can_See_If_A_Windows_Path_Has_An_Extension(string fullPath, bool expected)
            {
                // Given, When
                var path = new FilePath(fullPath);

                // Then
                path.HasExtension.ShouldBe(expected);
            }
        }

        public sealed class TheGetExtensionMethod
        {
            [Theory]
            [InlineData("assets/shaders/basic.frag", ".frag")]
            [InlineData("assets/shaders/basic.frag/test.vert", ".vert")]
            [InlineData("assets/shaders/basic.frag/test.foo.vert", ".vert")]
            [InlineData("assets/shaders/basic", null)]
            [InlineData("assets/shaders/basic.frag/test", null)]
            public void Can_Get_Extension(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetExtension();

                // Then
                result.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/foo/bar/baz.txt", ".txt")]
            [InlineData("C:/foo/bar/baz.txt/qux.md", ".md")]
            [InlineData("C:/foo/bar/baz.txt/qux.md.rs", ".rs")]
            [InlineData("C:/foo/bar/baz", null)]
            [InlineData("C:/foo/bar/baz.txt/qux", null)]
            [InlineData(@"\\foo\bar\baz.txt", ".txt")]
            [InlineData(@"\\foo\bar\baz.txt\qux.md", ".md")]
            [InlineData(@"\\foo\bar\baz.txt\qux.md.rs", ".rs")]
            [InlineData(@"\\foo\bar\baz", null)]
            [InlineData(@"\\foo\bar\baz.txt\qux", null)]
            public void Can_Get_Windows_Extension(string fullPath, string expected)
            {
                // Given, When
                var path = new FilePath(fullPath);

                // When
                var result = path.GetExtension();

                // Then
                result.ShouldBe(expected);
            }
        }

        public sealed class TheRemoveExtensionMethod
        {
            [Theory]
            [InlineData("assets/shaders/basic.frag", "assets/shaders/basic")]
            [InlineData("assets/shaders/basic.frag/test.vert", "assets/shaders/basic.frag/test")]
            [InlineData("assets/shaders/basic.frag/test.foo.vert", "assets/shaders/basic.frag/test.foo")]
            [InlineData("assets/shaders/basic", "assets/shaders/basic")]
            [InlineData("assets/shaders/basic.frag/test", "assets/shaders/basic.frag/test")]
            public void Should_Remove_Extension(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.RemoveExtension();

                // Then
                result.FullPath.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/foo/bar/baz.txt", "C:/foo/bar/baz")]
            [InlineData("C:/foo/bar/baz.txt/qux.md", "C:/foo/bar/baz.txt/qux")]
            [InlineData("C:/foo/bar/baz.txt/qux.md.rs", "C:/foo/bar/baz.txt/qux.md")]
            [InlineData("C:/foo/bar/baz", "C:/foo/bar/baz")]
            [InlineData("C:/foo/bar/baz.txt/qux", "C:/foo/bar/baz.txt/qux")]
            [InlineData(@"\\foo\bar\baz.txt", @"\\foo\bar\baz")]
            [InlineData(@"\\foo\bar\baz.txt\qux.md", @"\\foo\bar\baz.txt\qux")]
            [InlineData(@"\\foo\bar\baz.txt\qux.md.rs", @"\\foo\bar\baz.txt\qux.md")]
            [InlineData(@"\\foo\bar\baz", @"\\foo\bar\baz")]
            [InlineData(@"\\foo\bar\baz.txt\qux", @"\\foo\bar\baz.txt\qux")]
            public void Should_Remove_Windows_Extension(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.RemoveExtension();

                // Then
                result.FullPath.ShouldBe(expected);
            }
        }

        public sealed class TheGetDirectoryMethod
        {
            [Theory]
            [InlineData("temp/hello.txt", "temp")]
            public void Can_Get_Directory_For_File_Path(string input, string expected)
            {
                // Given
                var path = new FilePath(input);

                // When
                var result = path.GetDirectory();

                // Then
                result.FullPath.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/temp/hello.txt", "C:/temp")]
            [InlineData(@"\\temp\hello.txt", @"\\temp")]
            public void Can_Get_Directory_For_Windows_File_Path(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetDirectory();

                // Then
                result.FullPath.ShouldBe(expected);
            }

            [Fact]
            public void Can_Get_Directory_For_Relative_File_Path_In_Root()
            {
                // Given
                var path = new FilePath("hello.txt");

                // When
                var result = path.GetDirectory();

                // Then
                result.FullPath.ShouldBeEmpty();
            }

            [Fact]
            public void Can_Get_Directory_For_Absolute_File_Path_In_Root()
            {
                // Given
                var path = new FilePath("/hello.txt");

                // When
                var result = path.GetDirectory();

                // Then
                result.FullPath.ShouldBe("/");
            }

            [WindowsTheory]
            [InlineData("C:/hello.txt", "C:/")]
            [InlineData(@"\\hello.txt", @"\\")]
            public void Can_Get_Directory_For_Absolute_File_Path_In_Windows_Root(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetDirectory();

                // Then
                result.FullPath.ShouldBe(expected);
            }
        }

        public sealed class TheChangeExtensionMethod
        {
            [Theory]
            [InlineData("temp/hello.txt", ".dat", "temp/hello.dat")]
            [InlineData("temp/hello", ".dat", "temp/hello.dat")]
            [InlineData("./", ".dat", "")]
            [InlineData("temp/hello.txt", null, "temp/hello")]
            [InlineData("temp/hello.txt", "", "temp/hello.")]
            [InlineData("temp/hello.txt", ".", "temp/hello.")]
            public void Can_Change_Extension_Of_Path(string input, string extension, string expected)
            {
                // Given
                var path = new FilePath(input);

                // When
                path = path.ChangeExtension(extension);

                // Then
                path.ToString().ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/temp/hello.txt", ".dat", "C:/temp/hello.dat")]
            [InlineData("C:/temp/hello", ".dat", "C:/temp/hello.dat")]
            [InlineData("C:/", ".dat", "C:/.dat")]
            [InlineData("C:/temp/hello.txt", null, "C:/temp/hello")]
            [InlineData("C:/temp/hello.txt", "", "C:/temp/hello.")]
            [InlineData("C:/temp/hello.txt", ".", "C:/temp/hello.")]
            public void Can_Change_Extension_Of_Windows_Path(string input, string extension, string expected)
            {
                // Given
                var path = new FilePath(input);

                // When
                path = path.ChangeExtension(extension);

                // Then
                path.ToString().ShouldBe(expected);
            }
        }

        public sealed class TheAppendExtensionMethod
        {
            [Fact]
            public void Should_Throw_If_Extension_Is_Null()
            {
                // Given
                var path = new FilePath("temp/hello.txt");

                // When
                var result = Record.Exception(() => path.AppendExtension(null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("extension");
            }

            [Theory]
            [InlineData("dat", "temp/hello.txt.dat")]
            [InlineData(".dat", "temp/hello.txt.dat")]
            public void Can_Append_Extension_To_Path(string extension, string expected)
            {
                // Given
                var path = new FilePath("temp/hello.txt");

                // When
                path = path.AppendExtension(extension);

                // Then
                path.ToString().ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/temp/hello.txt", ".dat", "C:/temp/hello.txt.dat")]
            [InlineData(@"\\temp\hello.txt", ".dat", @"\\temp\hello.txt.dat")]
            public void Can_Append_Extension_To_Windows_Path(string fullPath, string extension, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                path = path.AppendExtension(extension);

                // Then
                path.ToString().ShouldBe(expected);
            }
        }

        public sealed class TheGetFilenameMethod
        {
            [Theory]
            [InlineData("/input/test.txt", "test.txt")]
            [InlineData("/input/test.foo.txt", "test.foo.txt")]
            [InlineData("/input/test", "test")]
            [InlineData("/test.txt", "test.txt")]
            [InlineData("/test.foo.txt", "test.foo.txt")]
            [InlineData("./test.txt", "test.txt")]
            [InlineData("./test.foo.txt", "test.foo.txt")]
            [InlineData("./", "")]
            [InlineData("/", "")]
            public void Can_Get_Filename_From_Path(string input, string expected)
            {
                // Given
                var path = new FilePath(input);

                // When
                var result = path.GetFilename();

                // Then
                result.FullPath.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/input/test.txt", "test.txt")]
            [InlineData("C:/input/test.foo.txt", "test.foo.txt")]
            [InlineData("C:/input/test", "test")]
            [InlineData("C:/test.txt", "test.txt")]
            [InlineData("C:/test.foo.txt", "test.foo.txt")]
            [InlineData("C:/", "")]
            [InlineData(@"\\input\test.txt", "test.txt")]
            [InlineData(@"\\input\test.foo.txt", "test.foo.txt")]
            [InlineData(@"\\input\test", "test")]
            [InlineData(@"\\test.txt", "test.txt")]
            [InlineData(@"\\test.foo.txt", "test.foo.txt")]
            [InlineData(@"\\", "")]
            public void Can_Get_Filename_From_Windows_Path(string input, string expected)
            {
                // Given
                var path = new FilePath(input);

                // When
                var result = path.GetFilename();

                // Then
                result.FullPath.ShouldBe(expected);
            }
        }

        public sealed class TheGetFilenameWithoutExtensionMethod
        {
            [Theory]
            [InlineData("/input/test.txt", "test")]
            [InlineData("/input/test.foo.txt", "test.foo")]
            [InlineData("/input/test", "test")]
            [InlineData("/test.txt", "test")]
            [InlineData("/test.foo.txt", "test.foo")]
            [InlineData("./test.txt", "test")]
            [InlineData("./test.foo.txt", "test.foo")]
            [InlineData("./", "")]
            public void Should_Return_Filename_Without_Extension_From_Path(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetFilenameWithoutExtension();

                // Then
                result.FullPath.ShouldBe(expected);
            }

            [WindowsTheory]
            [InlineData("C:/input/test.txt", "test")]
            [InlineData("C:/input/test.foo.txt", "test.foo")]
            [InlineData("C:/input/test", "test")]
            [InlineData("C:/test.txt", "test")]
            [InlineData("C:/test.foo.txt", "test.foo")]
            [InlineData("C:/", "")]
            [InlineData(@"\\input\test.txt", "test")]
            [InlineData(@"\\input\test.foo.txt", "test.foo")]
            [InlineData(@"\\input\test", "test")]
            [InlineData(@"\\test.txt", "test")]
            [InlineData(@"\\test.foo.txt", "test.foo")]
            [InlineData(@"\\", "")]
            public void Should_Return_Filename_Without_Extension_From_Windows_Path(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetFilenameWithoutExtension();

                // Then
                result.FullPath.ShouldBe(expected);
            }
        }

        public sealed class TheMakeAbsoluteMethod
        {
            public sealed class WithEnvironment
            {
                [Fact]
                public void Should_Throw_If_Environment_Is_Null()
                {
                    // Given
                    var path = new FilePath("temp/hello.txt");

                    // When
                    var result = Record.Exception(() => path.MakeAbsolute((IEnvironment)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("environment");
                }

                [Fact]
                public void Should_Return_A_Absolute_File_Path_If_File_Path_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var environment = Substitute.For<IEnvironment>();
                    environment.WorkingDirectory.Returns(new DirectoryPath("/absolute"));

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    result.FullPath.ShouldBe("/absolute/test.txt");
                }

                [Theory]
                [InlineData("/test.txt")]
                public void Should_Return_Same_File_Path_If_File_Path_Is_Absolute(string expected)
                {
                    // Given
                    var path = new FilePath(expected);
                    var environment = Substitute.For<IEnvironment>();
                    environment.WorkingDirectory.Returns(new DirectoryPath("/absolute"));

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    result.FullPath.ShouldBe(expected);
                }

                [WindowsTheory]
                [InlineData("C:/foo/bar.txt")]
                [InlineData(@"\\foo\bar.txt")]
                public void Should_Create_New_Absolute_Windows_Path_Identical_To_The_Path(string expected)
                {
                    // Given
                    var path = new FilePath(expected);
                    var environment = Substitute.For<IEnvironment>();
                    environment.WorkingDirectory.Returns(new DirectoryPath("/absolute"));

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    result.FullPath.ShouldBe(expected);
                    result.ShouldNotBeSameAs(path);
                }

                [Fact]
                public void Should_Expand_Home_Directory()
                {
                    // Given
                    var path = new FilePath("~/test.txt");
                    var environment = new FakeEnvironment(PlatformFamily.Linux);

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    result.FullPath.ShouldBe("/home/Patrik/test.txt");
                }
            }

            public sealed class WithDirectoryPath
            {
                [Fact]
                public void Should_Throw_If_Provided_Directory_Is_Null()
                {
                    // Given
                    var path = new FilePath("./test.txt");

                    // When
                    var result = Record.Exception(() => path.MakeAbsolute((DirectoryPath)null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("path");
                }

                [Fact]
                public void Should_Throw_If_Provided_Directory_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var directory = new DirectoryPath("./relative");

                    // When
                    var result = Record.Exception(() => path.MakeAbsolute(directory));

                    // Then
                    result.ShouldBeOfType<InvalidOperationException>()
                        .And().Message.ShouldBe("Cannot make a file path absolute with a relative directory path.");
                }

                [Fact]
                public void Should_Return_A_Absolute_File_Path_If_File_Path_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var directory = new DirectoryPath("/absolute");

                    // When
                    var result = path.MakeAbsolute(directory);

                    // Then
                    result.FullPath.ShouldBe("/absolute/test.txt");
                }

                [Fact]
                public void Should_Return_Same_File_Path_If_File_Path_Is_Absolute()
                {
                    // Given
                    var path = new FilePath("/test.txt");
                    var directory = new DirectoryPath("/absolute");

                    // When
                    var result = path.MakeAbsolute(directory);

                    // Then
                    result.FullPath.ShouldBe("/test.txt");
                }
            }
        }

        public sealed class TheCollapseMethod
        {
            [Fact]
            public void Should_Collapse_Relative_Path()
            {
                // Given, When
                var path = new FilePath("hello/temp/test/../../world/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("hello/world/foo.txt");
            }

            [Fact]
            public void Should_Collapse_Path_With_Separated_Ellipsis()
            {
                // Given, When
                var path = new FilePath("hello/temp/../temp2/../world/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("hello/world/foo.txt");
            }

            [WindowsFact]
            public void Should_Collapse_Path_With_Windows_Root()
            {
                // Given, When
                var path = new FilePath("c:/hello/temp/test/../../world/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("c:/hello/world/foo.txt");
            }

            [Fact]
            public void Should_Collapse_Path_With_Non_Windows_Root()
            {
                // Given, When
                var path = new FilePath("/hello/temp/test/../../world/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("/hello/world/foo.txt");
            }

            [WindowsFact]
            public void Should_Stop_Collapsing_When_Windows_Root_Is_Reached()
            {
                // Given, When
                var path = new FilePath("c:/../../../../../../temp/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("c:/temp/foo.txt");
            }

            [Fact]
            public void Should_Stop_Collapsing_When_Root_Is_Reached()
            {
                // Given, When
                var path = new FilePath("/hello/../../../../../../temp/foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("/temp/foo.txt");
            }

            [Theory]
            [InlineData(".")]
            [InlineData("./")]
            [InlineData("/.")]
            public void Should_Collapse_Single_Dot_To_Single_Dot(string uncollapsedPath)
            {
                // Given, When
                var path = new FilePath(uncollapsedPath).Collapse();

                // Then
                path.FullPath.ShouldBe(".");
            }

            [Fact]
            public void Should_Collapse_Single_Dot_With_Ellipsis()
            {
                // Given, When
                var path = new FilePath("./../foo.txt").Collapse();

                // Then
                path.FullPath.ShouldBe("foo.txt");
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
            [InlineData(PlatformFamily.Windows, "~", "C:/Users/Patrik")]
            [InlineData(PlatformFamily.Windows, "~/", "C:/Users/Patrik")]
            [InlineData(PlatformFamily.Windows, "~/lol", "C:/Users/Patrik/lol")]
            [InlineData(PlatformFamily.Linux, "", "")]
            [InlineData(PlatformFamily.Linux, "~", "/home/Patrik")]
            [InlineData(PlatformFamily.Linux, "~/", "/home/Patrik")]
            [InlineData(PlatformFamily.Linux, "~/lol", "/home/Patrik/lol")]
            public void Should_Expand_Home_Directory_If_First_In_Path(PlatformFamily family, string input, string expected)
            {
                // Given
                var environment = new FakeEnvironment(family);
                var path = new FilePath(input);

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
                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/B/C", ".")]
                    [InlineData("C:/hello.txt", "C:/", ".")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/D/E", "../../D/E")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/", "../../..")]
                    [InlineData("C:/A/B/C/D/E/F/hello.txt", "C:/A/B/C", "../../..")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/B/C/D/E/F", "D/E/F")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\B\C", ".")]
                    [InlineData(@"\\hello.txt", @"\\", ".")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\D\E", "../../D/E")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\", "../../..")]
                    [InlineData(@"\\A\B\C\D\E\F\hello.txt", @"\\A\B\C", "../../..")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\B\C\D\E\F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        relativePath.FullPath.ShouldBe(expected);
                    }

                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt", "D:/A/B/C")]
                    [InlineData("C:/A/B/hello.txt", "D:/E/")]
                    [InlineData("C:/hello.txt", "B:/")]
                    [InlineData(@"\\A\B\C\hello.txt", "D:/A/B/C")]
                    [InlineData(@"\\A\B\hello.txt", "D:/E/")]
                    [InlineData(@"\\hello.txt", "B:/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath(to)));

                        // Then
                        result.ShouldBeOfType<InvalidOperationException>()
                            .And().Message.ShouldBe("Paths must share a common prefix.");
                    }

                    [WindowsFact]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Null()
                    {
                        // Given
                        var path = new FilePath("C:/A/B/C/hello.txt");

                        // When
                        var result = Record.Exception(() => path.GetRelativePath((DirectoryPath)null));

                        // Then
                        result.ShouldBeOfType<ArgumentNullException>()
                            .And().ParamName.ShouldBe("to");
                    }

                    [WindowsFact]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

                        // When
                        var result = Record.Exception(() => path.GetRelativePath(new DirectoryPath("C:/D/E/F")));

                        // Then
                        result.ShouldBeOfType<InvalidOperationException>()
                            .And().Message.ShouldBe("Source path must be an absolute path.");
                    }

                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt")]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Relative(string input)
                    {
                        // Given
                        var path = new FilePath(input);

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
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/B/C", ".")]
                    [InlineData("/C/hello.txt", "/C/", ".")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/D/E", "../../D/E")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/", "../../..")]
                    [InlineData("/C/A/B/C/D/E/F/hello.txt", "/C/A/B/C", "../../..")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/B/C/D/E/F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        relativePath.FullPath.ShouldBe(expected);
                    }

                    [Theory]
                    [InlineData("/C/A/B/C/hello.txt", "/D/A/B/C")]
                    [InlineData("/C/A/B/hello.txt", "/D/E/")]
                    [InlineData("/C/hello.txt", "/B/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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
                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/B/C/hello.txt", "hello.txt")]
                    [InlineData("C:/hello.txt", "C:/hello.txt", "hello.txt")]
                    [InlineData("C:/hello.txt", "C:/world.txt", "world.txt")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/hello.txt", "../../../hello.txt")]
                    [InlineData("C:/A/B/C/D/E/F/hello.txt", "C:/A/B/C/hello.txt", "../../../hello.txt")]
                    [InlineData("C:/A/B/C/hello.txt", "C:/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\B\C\hello.txt", "hello.txt")]
                    [InlineData(@"\\hello.txt", @"\\hello.txt", "hello.txt")]
                    [InlineData(@"\\hello.txt", @"\\world.txt", "world.txt")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\D\E\hello.txt", "../../D/E/hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\hello.txt", "../../../hello.txt")]
                    [InlineData(@"\\A\B\C\D\E\F\hello.txt", @"\\A\B\C\hello.txt", "../../../hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt", @"\\A\B\C\D\E\F\hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        relativePath.FullPath.ShouldBe(expected);
                    }

                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt", "D:/A/B/C/hello.txt")]
                    [InlineData("C:/A/B/hello.txt", "D:/E/hello.txt")]
                    [InlineData("C:/hello.txt", "B:/hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt", "D:/A/B/C/hello.txt")]
                    [InlineData(@"\\A\B\hello.txt", "D:/E/hello.txt")]
                    [InlineData(@"\\hello.txt", "B:/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var result = Record.Exception(() => path.GetRelativePath(new FilePath(to)));

                        // Then
                        result.ShouldBeOfType<InvalidOperationException>()
                            .And().Message.ShouldBe("Paths must share a common prefix.");
                    }

                    [WindowsFact]
                    public void Should_Throw_If_Target_FilePath_Is_Null()
                    {
                        // Given
                        var path = new FilePath("C:/A/B/C/hello.txt");

                        // When
                        var result = Record.Exception(() => path.GetRelativePath((FilePath)null));

                        // Then
                        result.ShouldBeOfType<ArgumentNullException>()
                            .And().ParamName.ShouldBe("to");
                    }

                    [WindowsFact]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

                        // When
                        var result = Record.Exception(() => path.GetRelativePath(new FilePath("C:/D/E/F/hello.txt")));

                        // Then
                        result.ShouldBeOfType<InvalidOperationException>()
                            .And().Message.ShouldBe("Source path must be an absolute path.");
                    }

                    [WindowsTheory]
                    [InlineData("C:/A/B/C/hello.txt")]
                    [InlineData(@"\\A\B\C\hello.txt")]
                    public void Should_Throw_If_Target_FilePath_Is_Relative(string input)
                    {
                        // Given
                        var path = new FilePath(input);

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
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/B/C/hello.txt", "hello.txt")]
                    [InlineData("/C/hello.txt", "/C/hello.txt", "hello.txt")]
                    [InlineData("/C/hello.txt", "/C/world.txt", "world.txt")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/hello.txt", "../../../hello.txt")]
                    [InlineData("/C/A/B/C/D/E/F/hello.txt", "/C/A/B/C/hello.txt", "../../../hello.txt")]
                    [InlineData("/C/A/B/C/hello.txt", "/C/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        relativePath.FullPath.ShouldBe(expected);
                    }

                    [Theory]
                    [InlineData("/C/A/B/C/hello.txt", "/D/A/B/C/hello.txt")]
                    [InlineData("/C/A/B/hello.txt", "/D/E/hello.txt")]
                    [InlineData("/C/hello.txt", "/B/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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
}