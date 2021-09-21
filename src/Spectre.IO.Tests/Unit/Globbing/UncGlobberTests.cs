using Shouldly;
using Spectre.IO.Tests.Fixtures;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO.Globbing
{
    public sealed class UncGlobberTests
    {
        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Ending_With_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\**\*");

            // Then
            result.Length.ShouldBe(15);
            result.ShouldContainDirectoryPath(@"\\Server\Foo");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Bar");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Baz");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Bar\Baz");
            result.ShouldContainDirectoryPath(@"\\Server\Bar");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qex.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.h");
            result.ShouldContainFilePath(@"\\Server\Foo\Baz\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Baz\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo.Bar.Test.dll");
            result.ShouldContainFilePath(@"\\Server\Bar.Qux.Test.dll");
            result.ShouldContainFilePath(@"\\Server\Quz.FooTest.dll");
            result.ShouldContainFilePath(@"\\Server\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Bar\Qux.h");
        }

        [Fact]
        public void Should_Throw_If_No_Share_Name_Has_Been_Specified()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = Record.Exception(() => fixture.Match(@"\\"));

            // Then
            result.ShouldNotBeNull();
            result.Message.ShouldBe(@"The pattern '\\' has no server part specified.");
        }

        [Theory]
        [InlineData(@"\\fo?")]
        [InlineData(@"\\fo*")]
        [InlineData(@"\\fo?\bar")]
        [InlineData(@"\\fo*\bar")]
        public void Should_Throw_If_Invalid_Share_Name_Has_Been_Specified(string input)
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = Record.Exception(() => fixture.Match(input));

            // Then
            result.ShouldNotBeNull();
            result.Message.ShouldBe($"The pattern '{input}' has an invalid server part specified.");
        }

        [Fact]
        public void Can_Traverse_Recursively()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\**\*.c");

            // Then
            result.Length.ShouldBe(5);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Baz\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qex.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Baz/Qux.c");
            result.ShouldContainFilePath(@"\\Server\Bar\Qux.c");
        }

        [Fact]
        public void Should_Be_Able_To_Visit_Parent_Using_Double_Dots()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\..\Foo\Bar\Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
        }

        [Fact]
        public void Should_Return_Single_Path_For_Absolute_File_Path_Without_Glob_Pattern()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\Bar\Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
        }

        [Fact]
        public void Should_Return_Single_Path_For_Absolute_Directory_Path_Without_Glob_Pattern()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\Bar");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Bar");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\*\Qux.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Baz\Qux.c");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Ending_With_Character_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\Bar\Q?x.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qex.c");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Character_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\Foo\Ba?\Qux.c");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Baz\Qux.c");
        }

        [Fact]
        public void Should_Return_Files_For_Pattern_Ending_With_Character_Wildcard_And_Dot()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\*.Test.dll");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath(@"\\Server\Foo.Bar.Test.dll");
            result.ShouldContainFilePath(@"\\Server\Bar.Qux.Test.dll");
        }

        [Fact]
        public void Should_Return_File_For_Recursive_Wildcard_Pattern_Ending_With_Wildcard_Regex()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\**\*.c");

            // Then
            result.Length.ShouldBe(5);
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Qex.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Baz\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Foo\Bar\Baz\Qux.c");
            result.ShouldContainFilePath(@"\\Server\Bar\Qux.c");
        }

        [Fact]
        public void Should_Return_Only_Folders_For_Pattern_Ending_With_Recursive_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\**");

            // Then
            result.Length.ShouldBe(6);
            result.ShouldContainDirectoryPath(@"\\Server");
            result.ShouldContainDirectoryPath(@"\\Server\Foo");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Bar");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Baz");
            result.ShouldContainDirectoryPath(@"\\Server\Foo\Bar\Baz");
            result.ShouldContainDirectoryPath(@"\\Server\Bar");
        }

        [Fact]
        public void Should_Include_Files_In_Root_Folder_When_Using_Recursive_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Foo\**\Bar.baz");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\Foo\Bar.baz");
        }

        [Fact]
        public void Should_Include_Folder_In_Root_Folder_When_Using_Recursive_Wildcard()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Foo\**\Bar");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainDirectoryPath(@"\\Foo\Bar");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Parenthesis_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Foo (Bar)\Baz.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\Foo (Bar)\Baz.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_AtSign_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Foo@Bar\Baz.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\Foo@Bar\Baz.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Relative_Directory_Not_At_The_Beginning()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\Server\.\*.Test.dll");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath(@"\\Server\Foo.Bar.Test.dll");
            result.ShouldContainFilePath(@"\\Server\Bar.Qux.Test.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Unicode_Characters_And_Ending_With_Identifier()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\嵌套\**\文件.延期");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\嵌套\目录\文件.延期");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Unicode_Characters_And_Not_Ending_With_Identifier()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match(@"\\嵌套\**\文件.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath(@"\\嵌套\目录\文件.延期");
        }
    }
}
