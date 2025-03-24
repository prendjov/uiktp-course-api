using Microsoft.AspNetCore.Http;

namespace Infrastructure.Common.Extensions;

public static class HttpContextExtensions
{
    public static T GetOrSetItem<T>(this HttpContext context, Func<T> getter)
    {
        var key = typeof(T).Name;
        if (context.Items.ContainsKey(key))
            return (T)context!.Items[key]!;

        var item = getter();
        context.Items[key] = item;
        return item;
    }
}
