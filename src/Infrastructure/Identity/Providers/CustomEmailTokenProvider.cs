using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Providers;

public class CustomEmailTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public CustomEmailTokenProvider(
        IDataProtectionProvider dataProtectionProvider, 
        IOptions<DataProtectionTokenProviderOptions> options,
        ILogger<CustomEmailTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }

    public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        // Validate the token
        var securityStampValidator = await manager.GetSecurityStampAsync(user);
        var customToken = await manager.GenerateUserTokenAsync(user, Options.Name, purpose);

        return token == customToken;
    }
}
