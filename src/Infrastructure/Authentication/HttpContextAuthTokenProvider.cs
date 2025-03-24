using Application.Cores.Authentication;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Authentication;

public class HttpContextAuthTokenProvider : IAuthTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAuthTokenProvider(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> GetAccessToken()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return default;
        }

        //Avoid warning
        await Task.Delay(0);

        var accessToken = _httpContextAccessor.HttpContext.Request.Headers[AuthConstants.AccessTokenName];
        return accessToken;
    }
}
