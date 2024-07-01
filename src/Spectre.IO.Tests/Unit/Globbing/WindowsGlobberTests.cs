using Shouldly;
using Spectre.IO.Tests.Fixtures;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO.Globbing;

public sealed class WindowsGlobberTests
{
    public sealed class TheMatchMethod
    {
        [Fact]
        public void Will_Fix_Root_If_Drive_Is_Missing_By_Using_The_Drive_From_The_Working_Directory()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("/Working/Foo/Bar/Qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Working/Foo/Bar/Qux.c");
        }

        [Fact]
        public void Should_Ignore_Case_Sensitivity_On_Case_Insensitive_Operative_System()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Working/**/qux.c");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Working/Foo/Bar/Qux.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Parenthesis_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Program Files (x86)/Foo.*");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Program Files (x86)/Foo.c");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Ampersand_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Tools & Services/*.dll");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Tools & Services/MyTool.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Plus_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Tools + Services/*.dll");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Tools + Services/MyTool.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Percent_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Some %2F Directory/*.dll");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Some %2F Directory/MyTool.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_Exclamation_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Some ! Directory/*.dll");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Some ! Directory/MyTool.dll");
        }

        [Fact]
        public void Should_Parse_Glob_Expressions_With_AtSign_In_Them()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Some@Directory/*.dll");

            // Then
            result.Length.ShouldBe(1);
            result.ShouldContainFilePath("C:/Some@Directory/MyTool.dll");
        }

        [Fact]
        public void Should_Return_Files_For_Pattern_Ending_With_Character_Wildcard_And_Dot_On_Windows()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Working/*.Test.dll");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("C:/Working/Project.A.Test.dll");
            result.ShouldContainFilePath("C:/Working/Project.B.Test.dll");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Bracket_Wildcard_On_Windows()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Working/fooba[rz].rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("C:/Working/foobar.rs");
            result.ShouldContainFilePath("C:/Working/foobaz.rs");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Brace_Expansion_On_Windows()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Working/foo{bar,bax}.rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("C:/Working/foobar.rs");
            result.ShouldContainFilePath("C:/Working/foobax.rs");
        }

        [Fact]
        public void Should_Return_Files_And_Folders_For_Pattern_Containing_Negated_Bracket_Wildcard_On_Windows()
        {
            // Given
            var fixture = GlobberFixture.Windows();

            // When
            var result = fixture.Match("C:/Working/fooba[!x].rs");

            // Then
            result.Length.ShouldBe(2);
            result.ShouldContainFilePath("C:/Working/foobar.rs");
            result.ShouldContainFilePath("C:/Working/foobaz.rs");
        }
    }
}