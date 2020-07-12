using System;
using Shouldly;
using Spectre.IO.Testing;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO
{
    public sealed class PathExtensionsTests
    {
        public sealed class TheExpandEnvironmentVariablesMethod
        {
            public sealed class ThatTakesAFilePath
            {
                [Fact]
                public void Should_Throw_If_Environment_Is_Null()
                {
                    // Given
                    var path = new FilePath("/%FOO%/baz.qux");

                    // When
                    var result = Record.Exception(() => path.ExpandEnvironmentVariables(null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("environment");
                }

                [Fact]
                public void Should_Expand_Existing_Environment_Variables()
                {
                    // Given
                    var environment = FakeEnvironment.CreateWindowsEnvironment();
                    environment.SetEnvironmentVariable("FOO", "bar");
                    var path = new FilePath("/%FOO%/baz.qux");

                    // When
                    var result = path.ExpandEnvironmentVariables(environment);

                    // Then
                    result.FullPath.ShouldBe("/bar/baz.qux");
                }
            }

            public sealed class ThatTakesADirectoryPath
            {
                [Fact]
                public void Should_Throw_If_Environment_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("/%FOO%/baz");

                    // When
                    var result = Record.Exception(() => path.ExpandEnvironmentVariables(null));

                    // Then
                    result.ShouldBeOfType<ArgumentNullException>()
                        .And().ParamName.ShouldBe("environment");
                }

                [Fact]
                public void Should_Expand_Existing_Environment_Variables()
                {
                    // Given
                    var environment = FakeEnvironment.CreateWindowsEnvironment();
                    environment.SetEnvironmentVariable("FOO", "bar");
                    var path = new DirectoryPath("/%FOO%/baz");

                    // When
                    var result = path.ExpandEnvironmentVariables(environment);

                    // Then
                    result.FullPath.ShouldBe("/bar/baz");
                }
            }
        }
    }
}
