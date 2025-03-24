using System.Diagnostics.Contracts;

namespace Application.Common.Extensions;

public static class EnumerableExtensions
{

    [Pure]
    public static T[] AsArray<T>(this IEnumerable<T> source)
    {
        return source as T[] ?? source.ToArray();
    }

    [Pure]
    public static bool HasValue<T>(this IEnumerable<T> source)
    {
        return source.Any();
    }

    [Pure]
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    [Pure]
    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    [Pure]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}
