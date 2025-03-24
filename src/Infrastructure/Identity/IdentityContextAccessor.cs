using Application.Identity;

namespace Infrastructure.Identity;

internal class IdentityContextAccessor : IIdentityContextAccessor
{
    private readonly IIdentityContext _identityContextDefault;
    private IIdentityContext? _identityContext;

    public IIdentityContext IdentityContext
    {
        get => _identityContext ?? _identityContextDefault;
        set => _identityContext = value;
    }

    public IdentityContextAccessor(IIdentityContextDefault identityContextDefault)
    {
        _identityContextDefault = identityContextDefault;
    }
}
