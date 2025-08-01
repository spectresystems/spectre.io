using Spectre.IO.Internal;

namespace Spectre.IO;

/// <summary>
/// The file system globber.
/// </summary>
[PublicAPI]
public sealed class Globber : IGlobber
{
    private readonly IPathComparer _comparer;
    private readonly GlobParser _parser;
    private readonly GlobVisitor _visitor;

    /// <summary>
    /// Gets the default <see cref="Globber"/> instance.
    /// </summary>
    public static Globber Shared { get; } = new Globber(FileSystem.Shared, Environment.Shared);

    /// <summary>
    /// Initializes a new instance of the <see cref="Globber"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="environment">The environment.</param>
    public Globber(IFileSystem fileSystem, IEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(environment);

        _comparer = fileSystem.Comparer;
        _parser = new GlobParser(environment);
        _visitor = new GlobVisitor(fileSystem, environment);
    }

    /// <inheritdoc/>
    public IEnumerable<Path> Match(string pattern, GlobberSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(pattern);

        if (string.IsNullOrWhiteSpace(pattern))
        {
            return [];
        }

        // Parse the pattern into an AST.
        var comparer = settings.Comparer ?? _comparer;
        var root = _parser.Parse(pattern, comparer);

        // Visit all nodes in the parsed patterns and filter the result.
        return _visitor.Walk(root, settings)
            .Select(x => x.Path)
            .Distinct(comparer);
    }
}