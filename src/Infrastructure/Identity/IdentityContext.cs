using Application.Identity;
using Infrastructure.Common.Extensions;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Identity;

internal sealed class IdentityContext : IIdentityContextDefault
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public IUserInfo CurrentUser => GetUser();

    private IUserInfo GetUser()
    {
        if (!IsAuthenticated) throw new UnauthorizedAccessException();
        return _httpContextAccessor.HttpContext!.GetOrSetItem(() => new UserInfo(_httpContextAccessor.HttpContext?.User!));
    }
}
