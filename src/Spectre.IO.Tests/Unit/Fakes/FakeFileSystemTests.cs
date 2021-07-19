using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.Fakes
{
    public sealed class FakeFileSystemTests
    {
        [Fact]
        public void Should_Create_Symbolic_Link()
        {
            // Given
            var environment = new FakeEnvironment(PlatformFamily.Linux);
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.CreateDirectory("/home/Patrik");
            fileSystem.CreateFile("/Working/test.txt");

            // When
            fileSystem.File.CreateSymbolicLink("/Working/test.txt", "/home/Patrik/foo.txt");
            var file = fileSystem.GetFakeFile("/home/Patrik/foo.txt");

            // Then
            file.ShouldNotBeNull();
            file.SymbolicLink.ShouldNotBeNull();
            file.SymbolicLink.Path.FullPath.ShouldBe("/Working/test.txt");
            file.Attributes.HasFlag(System.IO.FileAttributes.ReparsePoint).ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_Content_Of_Original_File()
        {
            // Given
            var environment = new FakeEnvironment(PlatformFamily.Linux);
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.CreateDirectory("/home/Patrik");
            fileSystem.CreateFile("/Working/test.txt").SetTextContent("LOL");
            fileSystem.File.CreateSymbolicLink("/Working/test.txt", "/home/Patrik/foo.txt");

            // When
            var result = fileSystem.GetFakeFile("/home/Patrik/foo.txt").GetTextContent();

            // Then
            result.ShouldBe("LOL");
        }
    }
}
