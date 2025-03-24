namespace Application.Common.Extensions;

public static class LanguageExtensions
{
    public static void Do<TIn>(this TIn a, Action<TIn> f) => f(a);

    public static TOut Apply<TIn, TOut>(this TIn a, Func<TIn, TOut> f) => f(a);

    public static async Task<TOut> ApplyAsync<TIn, TOut>(this Task<TIn> a, Func<TIn, TOut> f) => f(await a);

    public static async Task<TOut> ApplyAsync<TIn, TOut>(this Task<TIn> a, Func<TIn, Task<TOut>> f) => await f(await a);

    public static async Task<TOut> ApplyAsync<TIn, TOut>(this ValueTask<TIn> a, Func<TIn, Task<TOut>> f) => await f(await a);

    public static async Task<TOut> ApplyAsync<TIn, TOut>(this ValueTask<TIn> a, Func<TIn, ValueTask<TOut>> f) => await f(await a);
}
