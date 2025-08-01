namespace Spectre.IO.Testing;

/// <summary>
/// Contains extension methods for byte arrays.
/// </summary>
[PublicAPI]
public static class ByteArrayExtensions
{
    /// <summary>
    /// Determines if a byte array starts with a specified prefix.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="prefix">The prefix to compare.</param>
    /// <returns>Whether or not the byte array starts with the specified prefix.</returns>
    public static bool StartsWith(this byte[] value, byte[] prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        if (value.Length < prefix.Length)
        {
            return false;
        }

        return !prefix.Where((t, i) => value[i] != t).Any();
    }
}