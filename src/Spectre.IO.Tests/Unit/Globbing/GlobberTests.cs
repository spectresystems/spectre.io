using NSubstitute;
using Shouldly;
using Spectre.IO.Tests.Fixtures;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO.Globbing;

public sealed class GlobberTests
{
    public sealed class TheConstructor
    {
        [Fact]
        public void Should_Throw_If_File_System_Is_Null()
        {
            // Given, When
            var environment = Substitute.For<IEnvironment>();
            var result = Record.Exception(() => new Globber(null, environment));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("fileSystem");
        }

        [Fact]
        public void Should_Throw_If_Environment_Is_Null()
        {
            // Given
            var fileSystem = Substitute.For<IFileSystem>();

            // When
            var result = Record.Exception(() => new Globber(fileSystem, null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("environment");
        }
    }

    public sealed class TheMatchMethod
    {
        public sealed class WithDirectoryPredicate
        {
            [Fact]
            public void Should_Return_Paths_Not_Affected_By_Walker_Hints()
            {
                // Given
                var fixture = GlobberFixture.UnixLike();
                var predicate = new Func<IFileSystemInfo, bool>(i =>
                    i.Path.FullPath != "/Working/Bar");

                // When
                var result = fixture.Match("./**/Qux.h", predicate);

                // Then
                result.Length.ShouldBe(1);
                result.ShouldContainFilePath("/Working/Foo/Bar/Qux.h");
            }

            [Fact]
            public void Should_Not_Return_Path_If_Walker_Hint_Matches_Part_Of_Pattern()
            {
                // Given
                var fixture = GlobberFixture.UnixLike();
                var predicate = new Func<IFileSystemInfo, bool>(i =>
                    i.Path.FullPath != "/Working/Bar");

                // When
                var result = fixture.Match("/Working/Bar/Qux.h", predicate);

                // Then
                result.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Not_Return_Path_If_Walker_Hint_Exactly_Match_Pattern()
            {
                // Given
                var fixture = GlobberFixture.UnixLike();
                var predicate = new Func<IFileSystemInfo, bool>(i =>
                    i.Path.FullPath != "/Working/Bar");

                // When
                var result = fixture.Match("/Working/Bar", predicate);

                // Then
                result.ShouldBeEmpty();
            }
        }

        public sealed class WithFilePredicate
        {
            [Fact]
            public void Should_Return_Only_Files_Matching_Predicate()
            {
                // Given
                var fixture = GlobberFixture.UnixLike();
                var predicate =
                    new Func<IFile, bool>(i => i.Path.FullPath.EndsWith(".c", StringComparison.OrdinalIgnoreCase));

                // When
                var result = fixture.Match("/Working/**/*.*", null, predicate);

                // Then
                result.Length.ShouldBe(5);
                result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
                result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
                result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
                result.ShouldContainFilePath("/Working/Foo/Bar/Baz/Qux.c");
                result.ShouldContainFilePath("/Working/Bar/Qux.c");
            }
        }

        public sealed class WithDirectoryAndFilePredicate
        {
            [Fact]
            public void Should_Return_Only_Files_Matching_Predicate()
            {
                // Given
                var fixture = GlobberFixture.UnixLike();
                var directoryPredicate = new Func<IFileSystemInfo, bool>(i =>
                    i.Path.FullPath.Contains("/Working", StringComparison.OrdinalIgnoreCase));
                var filePredicate = new Func<IFile, bool>(i =>
                    !i.Path.FullPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

                // When
                var result = fixture.Match("./**/*.*", directoryPredicate, filePredicate);

                // Then
                result.Length.ShouldBe(10);
                result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
                result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
                result.ShouldContainFilePath("/Working/Foo/Bar/Qux.h");
                result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
                result.ShouldContainFilePath("/Working/Foo/Bar/Baz/Qux.c");
                result.ShouldContainFilePath("/Working/Bar/Qux.c");
                result.ShouldContainFilePath("/Working/Bar/Qux.h");
                result.ShouldContainFilePath("/Working/foobar.rs");
                result.ShouldContainFilePath("/Working/foobaz.rs");
                result.ShouldContainFilePath("/Working/foobax.rs");
            }
        }

        [Fact]
        public void Should_Throw_If_Pattern_Is_Null()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = Record.Exception(() => fixture.Match(null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("pattern");
        }

        [Fact]
        public void Should_Return_Empty_Result_If_Pattern_Is_Empty()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(string.Empty);

            // Then
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Return_Empty_Result_If_Pattern_Is_Invalid()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("pattern/");

            // Then
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Traverse_Recursively()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/**/*.c");

            // Then
            result.Length.ShouldBe(5);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Bar/Qux.c");
        }

        [Fact]
        public void Will_Append_Relative_Root_With_Implicit_Working_Directory()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("Foo/Bar/Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
        }

        [Fact]
        public void Should_Be_Able_To_Visit_Parent_Using_Double_Dots()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/Foo/../Foo/Bar/Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
        }

        [Fact]
        public void Should_Throw_If_Visiting_Parent_That_Is_Recursive_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = Record.Exception(() => fixture.Match("/Working/Foo/**/../Foo/Bar/Qux.c"));

            // Then
            result.ShouldBeOfType<NotSupportedException>()
                .And().Message.ShouldBe("Visiting a parent that is a recursive wildcard is not supported.");
        }

        [Theory]
        [InlineData("/RootFile.sh")]
        [InlineData("/Working/Foo/Bar/Qux.c")]
        public void Should_Return_Single_Path_For_Absolute_File_Path_Without_Glob_Pattern(string pattern)
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(pattern);

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(pattern);
        }

        [Theory]
        [InlineData("/RootDir")]
        [InlineData("/Working/Foo/Bar")]
        public void Should_Return_Single_Path_For_Absolute_Directory_Path_Without_Glob_Pattern(string pattern)
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(pattern);

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainDirectoryPath(pattern);
        }

        [Fact]
        public void Should_Return_Single_Path_For_Relative_File_Path_Without_Glob_Pattern()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();
            fixture.SetWorkingDirectory("/Working/Foo");

            // When
            var result = fixture.Match("./Bar/Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
        }

        [Fact]
        public void Should_Return_Single_Path_For_Relative_Directory_Path_Without_Glob_Pattern()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();
            fixture.SetWorkingDirectory("/Working/Foo");

            // When
            var result = fixture.Match("./Bar");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainDirectoryPath("/Working/Foo/Bar");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Ending_With_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/**/*");

            // Then
            result.Length.ShouldBe(18);
            result.ShouldContainDirectoryPath("/Working/Foo");
            result.ShouldContainDirectoryPath("/Working/Foo/Bar");
            result.ShouldContainDirectoryPath("/Working/Foo/Baz");
            result.ShouldContainDirectoryPath("/Working/Foo/Bar/Baz");
            result.ShouldContainDirectoryPath("/Working/Bar");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.h");
            result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Foo.Bar.Test.dll");
            result.ShouldContainFilePath("/Working/Bar.Qux.Test.dll");
            result.ShouldContainFilePath("/Working/Quz.FooTest.dll");
            result.ShouldContainFilePath("/Working/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Bar/Qux.h");
            result.ShouldContainFilePath("/Working/foobar.rs");
            result.ShouldContainFilePath("/Working/foobaz.rs");
            result.ShouldContainFilePath("/Working/foobax.rs");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/Foo/*/Qux.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Ending_With_Character_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/Foo/Bar/Q?x.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Character_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/Foo/Ba?/Qux.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
        }

        [Fact]
        public void Should_Return_Files_For_Pattern_Ending_With_Character_Wildcard_And_Dot()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/*.Test.dll");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/Foo.Bar.Test.dll");
            result.ShouldContainFilePath("/Working/Bar.Qux.Test.dll");
        }

        [Fact]
        public void Should_Return_File_For_Recursive_Wildcard_Pattern_Ending_With_Wildcard_Regex()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/**/*.c");

            // Then
            result.Length.ShouldBe(5);
            result.ShouldContainFilePath("/Working/Foo/Bar/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Qex.c");
            result.ShouldContainFilePath("/Working/Foo/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Foo/Bar/Baz/Qux.c");
            result.ShouldContainFilePath("/Working/Bar/Qux.c");
        }

        [Fact]
        public void Should_Return_Only_Folders_For_Pattern_Ending_With_Recursive_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/**");

            // Then
            result.Length.ShouldBe(6);
            result.ShouldContainDirectoryPath("/Working");
            result.ShouldContainDirectoryPath("/Working/Foo");
            result.ShouldContainDirectoryPath("/Working/Foo/Bar");
            result.ShouldContainDirectoryPath("/Working/Foo/Baz");
            result.ShouldContainDirectoryPath("/Working/Foo/Bar/Baz");
            result.ShouldContainDirectoryPath("/Working/Bar");
        }

        [Theory]
        [InlineData("/*.sh", "/RootFile.sh")]
        [InlineData("/Foo/*.baz", "/Foo/Bar.baz")]
        public void Should_Include_Files_In_Root_Folder_When_Using_Wildcard(string pattern, string file)
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(pattern);

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(file);
        }

        [Theory]
        [InlineData("/**/RootFile.sh", "/RootFile.sh")]
        [InlineData("/Foo/**/Bar.baz", "/Foo/Bar.baz")]
        public void Should_Include_Files_In_Root_Folder_When_Using_Recursive_Wildcard(string pattern, string file)
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(pattern);

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(file);
        }

        [Theory]
        [InlineData("/**/RootDir", "/RootDir")]
        [InlineData("/Foo/**/Bar", "/Foo/Bar")]
        public void Should_Include_Folder_In_Root_Folder_When_Using_Recursive_Wildcard(string pattern, string folder)
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match(pattern);

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainDirectoryPath(folder);
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Parenthesis_In_Them()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Foo (Bar)/Baz.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/Foo (Bar)/Baz.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_AtSign_In_Them()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Foo@Bar/Baz.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/Foo@Bar/Baz.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Relative_Directory_Not_At_The_Beginning()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/./*.Test.dll");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/Foo.Bar.Test.dll");
            result.ShouldContainFilePath("/Working/Bar.Qux.Test.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Unicode_Characters_And_Ending_With_Identifier()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/嵌套/**/文件.延期");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/嵌套/目录/文件.延期");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Unicode_Characters_And_Not_Ending_With_Identifier()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/嵌套/**/文件.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("/嵌套/目录/文件.延期");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Bracket_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/fooba[rz].rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/foobar.rs");
            result.ShouldContainFilePath("/Working/foobaz.rs");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Brace_Expansion()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/foo{bar,bax}.rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/foobar.rs");
            result.ShouldContainFilePath("/Working/foobax.rs");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Negated_Bracket_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("/Working/fooba[!x].rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/Working/foobar.rs");
            result.ShouldContainFilePath("/Working/foobaz.rs");
        }

        [Fact]
        public void Should_Return_Files_In_Home_Directory()
        {
            // Given
            var fixture = GlobberFixture.UnixLike();

            // When
            var result = fixture.Match("~/fooba[!x].rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("/home/JohnDoe/foobar.rs");
            result.ShouldContainFilePath("/home/JohnDoe/foobaz.rs");
        }
    }
}