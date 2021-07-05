namespace Spectre.IO.Internal
{
    internal enum GlobTokenKind
    {
        Wildcard,
        CharacterWildcard,
        PathSeparator,
        Text,
        HomeDirectory,
        WindowsRoot,
        Parent,
        Current,
        BracketWildcard,
        BraceExpansion,
    }
}