using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO;

public sealed class FileTests
{
    public sealed class ReadAllText
    {
        [Fact]
        public void Should_Read_Text()
        {
            // Given
            var machine = new FakeMachine(PlatformFamily.Linux);
            machine.FileSystem.CreateFile("/home/patrik/test.txt").SetTextContent("Hello World");

            // When
            var result = machine.FileSystem
                .GetFile("/home/patrik/test.txt")
                .ReadAllText();

            // Then
            result.ShouldBe("Hello World");
        }
    }

    public sealed class ReadAllTextAsync
    {
        [Fact]
        public async Task Should_Read_Text()
        {
            // Given
            var machine = new FakeMachine(PlatformFamily.Linux);
            machine.FileSystem.CreateFile("/home/patrik/test.txt").SetTextContent("Hello World");

            // When
            var result = await machine.FileSystem
                .GetFile("/home/patrik/test.txt")
                .ReadAllTextAsync();

            // Then
            result.ShouldBe("Hello World");
        }
    }

    public sealed class WriteAllText
    {
        [Fact]
        public void Should_Write_Text()
        {
            // Given
            var machine = new FakeMachine(PlatformFamily.Linux);
            var file = machine.FileSystem.CreateFile("/home/patrik/test.txt");

            // When
            file.WriteAllText("Hello World");

            // Then
            file.ReadAllText().ShouldBe("Hello World");
        }
    }

    public sealed class WriteAllTextAsync
    {
        [Fact]
        public async Task Should_Write_Text()
        {
            // Given
            var machine = new FakeMachine(PlatformFamily.Linux);
            var file = machine.FileSystem.CreateFile("/home/patrik/test.txt");

            // When
            await file.WriteAllTextAsync("Hello World");

            // Then
            file.ReadAllText().ShouldBe("Hello World");
        }
    }
}