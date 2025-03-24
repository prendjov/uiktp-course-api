using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Configuration;

public class UserPasswordConfig : PasswordOptions
{
    public const string SectionName = "UserPassword";
}
