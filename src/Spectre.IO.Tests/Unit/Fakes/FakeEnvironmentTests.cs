using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.Fakes;

public class FakeEnvironmentTests
{
    [Theory]
    [InlineData(PlatformFamily.Linux, "/tmp")]
    [InlineData(PlatformFamily.FreeBSD, "/tmp")]
    [InlineData(PlatformFamily.MacOs, "/var/folders/tmp")]
    [InlineData(PlatformFamily.Windows, "C:/Users/JohnDoe/AppData/Local/Temp")]
    public void Should_Return_Temp_Directory(PlatformFamily platform, string expected)
    {
        // Given
        var environment = new FakeEnvironment(platform);
        var fileSystem = new FakeFileSystem(environment);

        // When
        var result = environment.GetTempDirectory();

        // Then
        result.FullPath.ShouldBe(expected);
    }

    [Theory]
    [InlineData(PlatformFamily.Linux, "/tmp/00000001.tmp")]
    [InlineData(PlatformFamily.FreeBSD, "/tmp/00000001.tmp")]
    [InlineData(PlatformFamily.MacOs, "/var/folders/tmp/00000001.tmp")]
    [InlineData(PlatformFamily.Windows, "C:/Users/JohnDoe/AppData/Local/Temp/00000001.tmp")]
    public void Should_Create_Temp_File(PlatformFamily family, string expected)
    {
        // Given
        var environment = new FakeEnvironment(family);
        var fileSystem = new FakeFileSystem(environment);

        // When
        var result = environment.GetTempFile(fileSystem);

        // Then
        result.Exists.ShouldBeTrue();
        result.Path.FullPath.ShouldBe(expected);
    }
}