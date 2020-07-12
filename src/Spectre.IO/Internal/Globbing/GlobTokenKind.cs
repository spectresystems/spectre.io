namespace Spectre.IO.Internal
{
    internal enum GlobTokenKind
    {
        Wildcard,
        CharacterWildcard,
        PathSeparator,
        Text,
        WindowsRoot,
        Parent,
        Current,
        BracketWildcard,
        BraceExpansion,
    }
}