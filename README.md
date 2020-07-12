# Spectre.IO

Spectre.IO is a .NET library containing cross-platform abstractions and implementations for IO. It also comes with a library that includes an in-memory implementation of a 
file system to use when writing tests.

## Table of Contents

1. [Usage](#usage)
2. [Testing](#testing)
3. [Contributing](#building-from-source)
5. [License](#license)

## Usage

```csharp
using Spectre.IO;

public bool CheckSomething()
{
    // Get the full path to "foo/hello.txt" in the current working directory.
    // Note: A path has no knowledge of the actual file. It's just a convenient
    // way to work with file system paths since it does things as normalization
    // automatically.
    var root = new DirectoryPath("foo").MakeAbsolute(_environment);
    var path = root.CombineWithFilePath(new FilePath("hello.txt"));

    // Of course, we could also have done this in one step.
    path = new FilePath("foo/hello.txt").MakeAbsolute(_environment);

    // Now get a file system reference to the file and check if it exist.
    var file = _fileSystem.GetFile(path);
    if (file.Exists)
    {
        // The file exists.
        // Does the file not contain the word "goodbye"?
        var contents = GetFileContents(file);
        if (!contents?.Contains("goodbye") ?? false)
        {
            return true;
        }
    }

    // The file "foo/hello.txt" was not found. Is there a file in any 
    // directory that contains the word "ten" or "tea"?
    // That should return true as well.
    if (_globber.GetFiles("**/*te{n|a}*").Any())
    {
        return true;
    }

    return false;
}

private string GetFileContents(IFile file)
{
    using (var stream = file.OpenRead())
    using (var reader = new StreamReader(stream))
    {
        return reader.ReadToEnd();
    }
}
```

## Testing

One purpose of Spectre.IO is enabling writing tests against code that uses the filesystem in some way. To write tests for the code above, we'll use the FakeFileSystem in Spectre.IO.Testing, an in-memory implementation of a file system.

```csharp
using Spectre.IO.Testing;

[Fact]
public void Should_Return_False_If_Hello_File_Exists_But_Contains_The_Word_Goodbye()
{
    // Given
    var environment = FakeEnvironment.CreateUnixEnvironment();
    var filesystem = new FakeFileSystem(environment);
    var globber = new Globber(filesystem, environment);
    var checker = new Checker(filesystem, environment, globber);

    filesystem.CreateFile("foo/bar.txt");
    filesystem.CreateFile("bar/qux.txt");
    filesystem.CreateFile("foo/hello.txt").SetTextContent("Is this goodbye?");

    // When
    var result = checker.CheckSomething();

    // Then
    result.ShouldBeFalse();
}
```

## Building from source

We're using [Cake](https://github.com/cake-build/cake) as a local dotnet tool for building. 
So make sure that you've restored Cake by running the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
```

## License

Spectre.IO is provided as-is under the MIT license and is based upon the IO abstractions in 
[Cake](https://github.com/cake-build/cake) that [Patrik Svensson](https://github.com/patriksvensson) 
(the author of this library) previously implemented as part of that project.

For more information see LICENSE.
