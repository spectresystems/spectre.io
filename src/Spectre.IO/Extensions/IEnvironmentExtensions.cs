using System.Text.RegularExpressions;

namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="IEnvironment"/>.
/// </summary>
[PublicAPI]
public static class IEnvironmentExtensions
{
    private static readonly Regex _regex = new Regex("%(.*?)%");

    /// <summary>
    /// Expands the environment variables in the provided text.
    /// </summary>
    /// <example>
    /// <code>
    /// var expanded = environment.ExpandEnvironmentVariables("%APPDATA%/foo");
    /// </code>
    /// </example>
    /// <param name="environment">The environment.</param>
    /// <param name="text">A string containing the names of zero or more environment variables.</param>
    /// <returns>A string with each environment variable replaced by its value.</returns>
    public static string ExpandEnvironmentVariables(this IEnvironment environment, string text)
    {
        ArgumentNullException.ThrowIfNull(environment);

        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var variables = environment.GetEnvironmentVariables();

        var matches = _regex.Matches(text);
        foreach (Match? match in matches)
        {
            if (match != null)
            {
                var value = match.Groups[1].Value;
                if (variables.TryGetValue(value, out var variable))
                {
                    text = text.Replace(match.Value, variable);
                }
            }
        }

        return text;
    }
}