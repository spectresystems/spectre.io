using System;

namespace Spectre.IO;

/// <summary>
/// Contains settings used by the globber.
/// </summary>
public sealed class GlobberSettings
{
    /// <summary>
    /// Gets or sets the root directory.
    /// </summary>
    public DirectoryPath? Root { get; set; }

    /// <summary>
    /// Gets or sets the predicate used to filter directories based on file system information.
    /// </summary>
    public Func<IDirectory, bool>? Predicate { get; set; }

    /// <summary>
    /// Gets or sets the filter used to filter files based on file system information.
    /// </summary>
    public Func<IFile, bool>? FilePredicate { get; set; }

    /// <summary>
    /// Gets or sets the comparer to use.
    /// </summary>
    public PathComparer? Comparer { get; set; }
}