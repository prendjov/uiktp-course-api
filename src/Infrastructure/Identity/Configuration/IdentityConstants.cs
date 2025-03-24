using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity.Configuration;

public static class IdentityConstants
{
    public static class Policies
    {
        public const string ExamplePolicy = nameof(ExamplePolicy);
    }

    public static class Claims
    {
        public const string Id = "id";
        public const string Scope = "scope";
        public const string Permissions = "permissions";
        public const string UserName = "userName";
        public const string FirstName = "firstName";
        public const string LastName = "lastName";
    }

    public const string DefaultSecurityAlgorithm = SecurityAlgorithms.HmacSha256;
}
