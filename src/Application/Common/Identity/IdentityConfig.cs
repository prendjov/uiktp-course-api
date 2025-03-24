namespace Application.Identity;

public class IdentityConfig
{
    public static string SectionName = "Identity";
    public int RefreshTokenValidityInDays { get; set; }
    public TimeSpan RefreshTokenValidity => TimeSpan.FromDays(RefreshTokenValidityInDays);
    public int TokenValidityInHours { get; set; }
    public TimeSpan TokenValidity => TimeSpan.FromHours(TokenValidityInHours);
    public string Secret { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
}
