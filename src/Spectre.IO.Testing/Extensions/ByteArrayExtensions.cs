using System;

namespace Spectre.IO.Testing;

/// <summary>
/// Contains extension methods for byte arrays.
/// </summary>
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
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (prefix == null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        if (value.Length < prefix.Length)
        {
            return false;
        }

        for (int i = 0; i < prefix.Length; i++)
        {
            if (value[i] != prefix[i])
            {
                return false;
            }
        }

        return true;
    }
}