using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Shouldly;
using Spectre.IO.Internal;
using Xunit;

namespace Spectre.IO.Tests.Unit.IO;

public sealed class PathComparerTests
{
    public sealed class TheEqualsMethod
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Same_Asset_Instances_Is_Considered_Equal(bool isCaseSensitive)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);
            var path = new FilePath("shaders/basic.vert");

            // When
            var result = comparer.Equals(path, path);

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Two_Null_Paths_Are_Considered_Equal(bool isCaseSensitive)
        {
            // Given
            var comparer = new PathComparer(isCaseSensitive);

            // When
            var result = comparer.Equals(null, null);

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Paths_Are_Considered_Inequal_If_Any_Is_Null(bool isCaseSensitive)
        {
            // Given
            var comparer = new PathComparer(isCaseSensitive);

            // When
            var result = comparer.Equals(null, new FilePath("test.txt"));

            // Then
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Same_Paths_Are_Considered_Equal(bool isCaseSensitive)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("shaders/basic.vert");

            // Then
            comparer.Equals(first, second).ShouldBeTrue();
            comparer.Equals(second, first).ShouldBeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Different_Paths_Are_Not_Considered_Equal(bool isCaseSensitive)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("shaders/basic.frag");

            // Then
            comparer.Equals(first, second).ShouldBeFalse();
            comparer.Equals(second, first).ShouldBeFalse();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Same_Paths_But_Different_Casing_Are_Considered_Equal_Depending_On_Case_Sensitivity(
            bool isCaseSensitive, bool expected)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("SHADERS/BASIC.VERT");

            // Then
            comparer.Equals(first, second).ShouldBe(expected);
            comparer.Equals(second, first).ShouldBe(expected);
        }
    }

    public sealed class TheCompareMethod
    {
        [Fact]
        public void Should_Sort_Paths()
        {
            // Given
            var paths = new List<FilePath>
            {
                new("foo/bar/qux"),
                new("foo/bar/baz"),
                new("foo/bar"),
            };

            // When
            var result = paths
                .Order(new PathComparer(isCaseSensitive: false))
                .ToList();

            // Then
            result[0].FullPath.ShouldBe("foo/bar");
            result[1].FullPath.ShouldBe("foo/bar/baz");
            result[2].FullPath.ShouldBe("foo/bar/qux");
        }
    }

    public sealed class TheGetHashCodeMethod
    {
        [Fact]
        public void Should_Throw_If_Other_Path_Is_Null()
        {
            // Given
            var comparer = new PathComparer(true);

            // When
            var result = Record.Exception(() => comparer.GetHashCode(null));

            // Then
            result.ShouldBeOfType<ArgumentNullException>()
                .And().ParamName.ShouldBe("obj");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Same_Paths_Get_Same_Hash_Code(bool isCaseSensitive)
        {
            // Given
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("shaders/basic.vert");

            // When
            var firstHash = comparer.GetHashCode(first);
            var secondHash = comparer.GetHashCode(second);

            // Then
            firstHash.ShouldBe(secondHash);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Different_Paths_Get_Different_Hash_Codes(bool isCaseSensitive)
        {
            // Given
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("shaders/basic.frag");

            // When
            var firstHash = comparer.GetHashCode(first);
            var secondHash = comparer.GetHashCode(second);

            // Then
            firstHash.ShouldNotBe(secondHash);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Same_Paths_But_Different_Casing_Get_Same_Hash_Code_Depending_On_Case_Sensitivity(
            bool isCaseSensitive, bool expected)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);
            var first = new FilePath("shaders/basic.vert");
            var second = new FilePath("SHADERS/BASIC.VERT");

            // When
            var result = comparer.GetHashCode(first) == comparer.GetHashCode(second);

            // Then
            result.ShouldBe(expected);
        }
    }

    public sealed class TheDefaultProperty
    {
        [Fact]
        public void Should_Return_Correct_Comparer_Depending_On_Operative_System()
        {
            // Given
            var expected = EnvironmentHelper.IsUnix();

            // When
            var comparer = PathComparer.Default;

            // Then
            comparer.IsCaseSensitive.ShouldBe(expected);
        }
    }

    public sealed class TheIsCaseSensitiveProperty
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Return_Whether_Or_Not_The_Comparer_Is_Case_Sensitive(bool isCaseSensitive)
        {
            // Given, When
            var comparer = new PathComparer(isCaseSensitive);

            // Then
            comparer.IsCaseSensitive.ShouldBe(isCaseSensitive);
        }
    }
}