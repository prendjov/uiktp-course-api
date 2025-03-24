using Application.Identity;

namespace Infrastructure.Identity;

public record IdentityContextCustom(IUserInfo CurrentUser) : IIdentityContext;
