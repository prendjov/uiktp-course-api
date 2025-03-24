namespace Application.Identity;

public interface IIdentityContextAccessor
{
    IIdentityContext IdentityContext { get; set; }
}
