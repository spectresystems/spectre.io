#nullable enable

using Spectre.IO.Testing;

namespace Spectre.IO.Tests.Fixtures;

internal sealed class GlobberFixture
{
    public FakeFileSystem FileSystem { get; set; }
    public FakeEnvironment Environment { get; set; }

    private GlobberFixture(FakeMachine machine)
    {
        FileSystem = machine.FileSystem;
        Environment = machine.Environment;
    }

    public static GlobberFixture Windows()
    {
        var machine = FakeMachine.CreateWindowsMachine();

        // Directories
        machine.FileSystem.CreateDirectory("C://Working");
        machine.FileSystem.CreateDirectory("C://Working/Foo");
        machine.FileSystem.CreateDirectory("C://Working/Foo/Bar");
        machine.FileSystem.CreateDirectory("C:");
        machine.FileSystem.CreateDirectory("C:/Program Files (x86)");

        // UNC directories
        machine.FileSystem.CreateDirectory(@"\\Server");
        machine.FileSystem.CreateDirectory(@"\\Server\Foo");
        machine.FileSystem.CreateDirectory(@"\\Server\Foo\Bar");
        machine.FileSystem.CreateDirectory(@"\\Server\Bar");
        machine.FileSystem.CreateDirectory(@"\\Foo\Bar");
        machine.FileSystem.CreateDirectory(@"\\Foo (Bar)");
        machine.FileSystem.CreateDirectory(@"\\Foo@Bar\");
        machine.FileSystem.CreateDirectory(@"\\嵌套");
        machine.FileSystem.CreateDirectory(@"\\嵌套\目录");

        // Files
        machine.FileSystem.CreateFile("C:/Working/Foo/Bar/Qux.c");
        machine.FileSystem.CreateFile("C:/Program Files (x86)/Foo.c");
        machine.FileSystem.CreateFile("C:/Working/Project.A.Test.dll");
        machine.FileSystem.CreateFile("C:/Working/Project.B.Test.dll");
        machine.FileSystem.CreateFile("C:/Working/Project.IntegrationTest.dll");
        machine.FileSystem.CreateFile("C:/Tools & Services/MyTool.dll");
        machine.FileSystem.CreateFile("C:/Tools + Services/MyTool.dll");
        machine.FileSystem.CreateFile("C:/Some %2F Directory/MyTool.dll");
        machine.FileSystem.CreateFile("C:/Some ! Directory/MyTool.dll");
        machine.FileSystem.CreateFile("C:/Some@Directory/MyTool.dll");
        machine.FileSystem.CreateFile("C:/Working/foobar.rs");
        machine.FileSystem.CreateFile("C:/Working/foobaz.rs");
        machine.FileSystem.CreateFile("C:/Working/foobax.rs");

        // UNC files
        machine.FileSystem.CreateFile(@"\\Server\Foo/Bar/Qux.c");
        machine.FileSystem.CreateFile(@"\\Server\Foo/Bar/Qex.c");
        machine.FileSystem.CreateFile(@"\\Server\Foo/Bar/Qux.h");
        machine.FileSystem.CreateFile(@"\\Server\Foo/Baz/Qux.c");
        machine.FileSystem.CreateFile(@"\\Server\Foo/Bar/Baz/Qux.c");
        machine.FileSystem.CreateFile(@"\\Server\Bar/Qux.c");
        machine.FileSystem.CreateFile(@"\\Server\Bar/Qux.h");
        machine.FileSystem.CreateFile(@"\\Server\Foo.Bar.Test.dll");
        machine.FileSystem.CreateFile(@"\\Server\Bar.Qux.Test.dll");
        machine.FileSystem.CreateFile(@"\\Server\Quz.FooTest.dll");
        machine.FileSystem.CreateFile(@"\\Foo\Bar.baz");
        machine.FileSystem.CreateFile(@"\\Foo (Bar)\Baz.c");
        machine.FileSystem.CreateFile(@"\\Foo@Bar\Baz.c");
        machine.FileSystem.CreateFile(@"\\嵌套/目录/文件.延期");

        return new GlobberFixture(machine);
    }

    public static GlobberFixture UnixLike()
    {
        var machine = FakeMachine.CreateLinuxMachine();

        // Directories
        machine.FileSystem.CreateDirectory("/RootDir");
        machine.FileSystem.CreateDirectory("/home/JohnDoe");
        machine.FileSystem.CreateDirectory("/Working");
        machine.FileSystem.CreateDirectory("/Working/Foo");
        machine.FileSystem.CreateDirectory("/Working/Foo/Bar");
        machine.FileSystem.CreateDirectory("/Working/Bar");
        machine.FileSystem.CreateDirectory("/Foo/Bar");
        machine.FileSystem.CreateDirectory("/Foo (Bar)");
        machine.FileSystem.CreateDirectory("/Foo@Bar/");
        machine.FileSystem.CreateDirectory("/嵌套");
        machine.FileSystem.CreateDirectory("/嵌套/目录");

        // Files
        machine.FileSystem.CreateFile("/RootFile.sh");
        machine.FileSystem.CreateFile("/home/JohnDoe/foobar.rs");
        machine.FileSystem.CreateFile("/home/JohnDoe/foobaz.rs");
        machine.FileSystem.CreateFile("/home/JohnDoe/foobax.rs");
        machine.FileSystem.CreateFile("/Working/Foo/Bar/Qux.c");
        machine.FileSystem.CreateFile("/Working/Foo/Bar/Qex.c");
        machine.FileSystem.CreateFile("/Working/Foo/Bar/Qux.h");
        machine.FileSystem.CreateFile("/Working/Foo/Baz/Qux.c");
        machine.FileSystem.CreateFile("/Working/Foo/Bar/Baz/Qux.c");
        machine.FileSystem.CreateFile("/Working/Bar/Qux.c");
        machine.FileSystem.CreateFile("/Working/Bar/Qux.h");
        machine.FileSystem.CreateFile("/Working/Foo.Bar.Test.dll");
        machine.FileSystem.CreateFile("/Working/Bar.Qux.Test.dll");
        machine.FileSystem.CreateFile("/Working/Quz.FooTest.dll");
        machine.FileSystem.CreateFile("/Foo/Bar.baz");
        machine.FileSystem.CreateFile("/Foo (Bar)/Baz.c");
        machine.FileSystem.CreateFile("/Foo@Bar/Baz.c");
        machine.FileSystem.CreateFile("/嵌套/目录/文件.延期");
        machine.FileSystem.CreateFile("/Working/foobar.rs");
        machine.FileSystem.CreateFile("/Working/foobaz.rs");
        machine.FileSystem.CreateFile("/Working/foobax.rs");

        return new GlobberFixture(machine);
    }

    public void SetWorkingDirectory(DirectoryPath path)
    {
        Environment.SetWorkingDirectory(path);
    }

    public Path[] Match(
        string pattern,
        Func<IFileSystemInfo, bool>? directoryPredicate = null,
        Func<IFile, bool>? filePredicate = null)
    {
        return new Globber(FileSystem, Environment)
            .Match(pattern, new GlobberSettings
            {
                Predicate = directoryPredicate,
                FilePredicate = filePredicate,
            })
            .ToArray();
    }
}