using System;
using System.IO;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO
{
    public sealed class FileExtensionsTests
    {
        public sealed class TheOpenMethod
        {
            public sealed class WithFileMode
            {
                [Fact]
                public void Should_Throw_If_File_Is_Null()
                {
                    // Given, When
                    var result = Record.Exception(() => IFileExtensions.Open(null, FileMode.Create));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("file");
                }

                [Theory]
                [InlineData(FileMode.Append, FileAccess.Write)]
                [InlineData(FileMode.Create, FileAccess.ReadWrite)]
                [InlineData(FileMode.CreateNew, FileAccess.ReadWrite)]
                [InlineData(FileMode.Open, FileAccess.ReadWrite)]
                [InlineData(FileMode.OpenOrCreate, FileAccess.ReadWrite)]
                [InlineData(FileMode.Truncate, FileAccess.ReadWrite)]
                public void Should_Open_With_Specified_File_Mode_And_Infer_File_Access(FileMode mode, FileAccess access)
                {
                    // Given
                    var file = Substitute.For<IFile>();

                    // When
                    file.Open(mode);

                    // Then
                    file.Received(1).Open(mode, access, FileShare.None);
                }
            }

            public sealed class WithFileModeAndFileAccess
            {
                [Fact]
                public void Should_Throw_If_File_Is_Null()
                {
                    // Given, When
                    var result = Record.Exception(() => IFileExtensions.Open(null, FileMode.Create, FileAccess.Write));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("file");
                }

                [Theory]
                [InlineData(FileMode.Append, FileAccess.Write)]
                [InlineData(FileMode.Create, FileAccess.ReadWrite)]
                [InlineData(FileMode.CreateNew, FileAccess.ReadWrite)]
                [InlineData(FileMode.Open, FileAccess.ReadWrite)]
                [InlineData(FileMode.OpenOrCreate, FileAccess.ReadWrite)]
                [InlineData(FileMode.Truncate, FileAccess.ReadWrite)]
                public void Should_Open_With_Specified_File_Mode_And_Infer_File_Access(FileMode mode, FileAccess access)
                {
                    // Given
                    var file = Substitute.For<IFile>();

                    // When
                    file.Open(mode, access);

                    // Then
                    file.Received(1).Open(mode, access, FileShare.None);
                }
            }
        }

        public sealed class TheOpenReadMethod
        {
            [Fact]
            public void Should_Throw_If_File_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => IFileExtensions.OpenRead(null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("file");
            }

            [Fact]
            public void Should_Open_Stream_With_Expected_FileMode_And_FileAccess()
            {
                // Given
                var file = Substitute.For<IFile>();

                // When
                file.OpenRead();

                // Then
                file.Received(1).Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        public sealed class TheOpenWriteMethod
        {
            [Fact]
            public void Should_Throw_If_File_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => IFileExtensions.OpenWrite(null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .And().ParamName.ShouldBe("file");
            }

            [Fact]
            public void Should_Open_Stream_With_Expected_FileMode_And_FileAccess()
            {
                // Given
                var file = Substitute.For<IFile>();

                // When
                file.OpenWrite();

                // Then
                file.Received(1).Open(FileMode.Create, FileAccess.Write, FileShare.None);
            }
        }
    }
}