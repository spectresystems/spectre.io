using NSubstitute;
using Shouldly;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO;

public sealed class FileSystemExtensionsTest
{
    public sealed class TheExistMethod
    {
        public sealed class WithFilePath
        {
            [Fact]
            public void Should_Return_False_If_File_Do_Not_Exist()
            {
                // Given
                var fileSystem = Substitute.For<IFileSystem>();
                var file = Substitute.For<IFile>();
                file.Exists.Returns(false);
                fileSystem.File.Retrieve(Arg.Any<FilePath>()).Returns(file);

                // When
                var result = fileSystem.Exist((FilePath)"file.txt");

                // Then
                result.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_True_If_File_Exist()
            {
                // Given
                var fileSystem = Substitute.For<IFileSystem>();
                var file = Substitute.For<IFile>();
                file.Exists.Returns(true);
                fileSystem.File.Retrieve(Arg.Any<FilePath>()).Returns(file);

                // When
                var result = fileSystem.Exist((FilePath)"file.txt");

                // Then
                result.ShouldBeTrue();
            }

            [Fact]
            public void Should_Throw_If_FileSystem_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => IFileSystemExtensions.Exist(null, (FilePath)"file.txt"));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("fileSystem");
            }
        }

        public sealed class WithDirectoryPath
        {
            [Fact]
            public void Should_Return_False_If_Directory_Do_Not_Exist()
            {
                // Given
                var fileSystem = Substitute.For<IFileSystem>();
                var directory = Substitute.For<IDirectory>();
                directory.Exists.Returns(false);
                fileSystem.Directory.Retrieve(Arg.Any<DirectoryPath>()).Returns(directory);

                // When
                var result = fileSystem.Exist((DirectoryPath)"/Target");

                // Then
                result.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_True_If_Directory_Exist()
            {
                // Given
                var fileSystem = Substitute.For<IFileSystem>();
                var directory = Substitute.For<IDirectory>();
                directory.Exists.Returns(true);
                fileSystem.Directory.Retrieve(Arg.Any<DirectoryPath>()).Returns(directory);

                // When
                var result = fileSystem.Exist((DirectoryPath)"/Target");

                // Then
                result.ShouldBeTrue();
            }

            [Fact]
            public void Should_Throw_If_FileSystem_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => IFileSystemExtensions.Exist(null, (DirectoryPath)"/Target"));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("fileSystem");
            }
        }
    }
}