namespace Application.Identity;

public interface IIdentityContext
{
    IUserInfo CurrentUser { get; }
}

/// <summary>
/// Default identity context would be returned if custom context is not set by IIdentityContextAccessor
/// </summary>
public interface IIdentityContextDefault : IIdentityContext
{
}