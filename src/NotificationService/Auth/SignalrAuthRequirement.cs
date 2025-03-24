using Application.Identity;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;

namespace NotificationService.Auth
{
    public class SignalrAuthRequirement : AuthorizationHandler<SignalrAuthRequirement>, IAuthorizationRequirement
    {
        private readonly IIdentityContextAccessor _identityContextAccessor;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignalrAuthRequirement(
             IIdentityContextAccessor identityContextAccessor,
             IJwtTokenService jwtTokenService,
             IHttpContextAccessor httpContextAccessor)
        {
            _identityContextAccessor = identityContextAccessor;
            _jwtTokenService = jwtTokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SignalrAuthRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                if (!httpContext!.Request.Path.Value!.Contains("negotiate"))
                {
                    var token = httpContext.Request.Headers["Authorization"];

                    if (!string.IsNullOrEmpty(token))
                    {
                        var accessToken = token.ToString();

                        var claimsPrincipal = _jwtTokenService.GetPrincipalFromToken(accessToken, true);

                        if (claimsPrincipal == null)
                        {
                            context.Fail(new AuthorizationFailureReason(this, "Unauthorized"));
                            return Task.CompletedTask;
                        }

                        _httpContextAccessor.HttpContext!.User = claimsPrincipal;
                        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfo(claimsPrincipal!));
                        context.Succeed(requirement);
                        return Task.CompletedTask;

                    }
                    else
                    {
                        context.Fail(new AuthorizationFailureReason(this, "Unauthorized"));
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            else
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
    }

}
