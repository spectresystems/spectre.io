using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.Fakes;

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
        var result = fileSystem.GetFakeFile("/home/Patrik/foo.txt").ReadAllText();

        // Then
        result.ShouldBe("LOL");
    }

    [Fact]
    public void Should_Return_Last_Write_Time_Of_File()
    {
        // Given
        var environment = new FakeEnvironment(PlatformFamily.Linux);
        var fileSystem = new FakeFileSystem(environment);
        fileSystem.CreateDirectory("/home/Patrik");
        fileSystem.CreateFile("/home/Patrik/test.txt")
            .SetTextContent("LOL")
            .SetLastWriteTime(new DateTime(2023, 02, 19, 10, 41, 31));

        // When
        var result = fileSystem.GetFakeFile("/home/Patrik/test.txt")
            .LastWriteTime;

        // Then
        result.ShouldBe(new DateTime(2023, 02, 19, 10, 41, 31));
    }

    [Fact]
    public void Should_Return_Last_Write_Time_Of_Directory()
    {
        // Given
        var environment = new FakeEnvironment(PlatformFamily.Linux);
        var fileSystem = new FakeFileSystem(environment);
        fileSystem.CreateDirectory("/home/Patrik")
            .SetLastWriteTime(new DateTime(2023, 02, 19, 10, 41, 31));

        // When
        var result = fileSystem.GetFakeDirectory("/home/Patrik")
            .LastWriteTime;

        // Then
        result.ShouldBe(new DateTime(2023, 02, 19, 10, 41, 31));
    }

    [Fact]
    public void Should_Dump_Correct_Data()
    {
        // Given
        var environment = new FakeEnvironment(PlatformFamily.Linux);
        var fileSystem = new FakeFileSystem(environment);
        fileSystem.CreateDirectory("/home/Patrik");
        fileSystem.CreateDirectory("/home/Vale");
        fileSystem.CreateDirectory("/home/Ada");
        fileSystem.CreateFile("/home/Vale/love.you");
        fileSystem.CreateFile("/home/Ada/love.you");

        // When
        var result = fileSystem.ToString();

        // Then
        result.ShouldBe(
            """
            /home
                Ada
                    love.you
                Vale
                    love.you
                Patrik
            """);
    }
}