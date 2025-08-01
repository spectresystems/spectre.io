namespace Spectre.IO;

/// <summary>
/// Represents a search scope.
/// </summary>
[PublicAPI]
public enum SearchScope
{
    /// <summary>
    /// The current directory.
    /// </summary>
    Current,

    /// <summary>
    /// The current directory and child directories.
    /// </summary>
    Recursive,
}