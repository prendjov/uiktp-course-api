using System.Text.Json;

namespace Application.Common.Extensions;

public static class SerializationExtensions
{
    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null) => JsonSerializer.Serialize(obj, options);

    public static T? Deserialize<T>(this string value, JsonSerializerOptions? options = null) => value.HasValue() ? JsonSerializer.Deserialize<T>(value, options) : default;
}
