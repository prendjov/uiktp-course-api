using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace Application.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Indicates whatever the source string is not null, empty, or consists only of white-spaces.
    /// </summary>
    [Pure]
    public static bool HasValue([NotNullWhen(true)] this string? str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// Indicates whatever the source string is null or string.Empty string.
    /// </summary>
    [Pure]
    public static bool IsEmpty([NotNullWhen(false)] this string? source)
    {
        return !source.HasValue();
    }

    /// <summary>
    /// Join sequence of strings with given separator.
    /// </summary>
    [Pure]
    public static string JoinWith(this IEnumerable<string?> source, string separator = ", ")
    {
        return string.Join(separator, source);
    }

    public static string GetExtension(this string filePath) => Path.GetExtension(filePath);

    /// <summary>
    /// Indicates whatever the source string is valid JSON
    /// </summary>
    [Pure]
    public static bool IsJson(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            using var jsonDoc = JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
