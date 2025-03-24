using System.Diagnostics.Contracts;

namespace Application.Common.Extensions;

public static class TaskExtensions
{
    [Pure]
    public static Task<TA> AsTask<TA>(this TA self)
    {
        return Task.FromResult(self);
    }
}
