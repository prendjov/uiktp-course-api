using Application.Common.Authorizaion;
using Application.Common.Extensions;
using Application.Identity;
using Domain.Entities.User;
using Infrastructure.Identity.Configuration;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.DependencyInjection;

public static class IdentityModuleInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddConfigurationBoundOptions<IdentityConfig>(IdentityConfig.SectionName);

        services.Configure<IdentityOptions>(options =>
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

        var userPasswordConfig = configuration.GetSection(UserPasswordConfig.SectionName).Get<UserPasswordConfig>();

        services.AddIdentity<ApplicationUser, IdentityRole<int>>(
            options =>
            {
                options.Password.RequireDigit = userPasswordConfig!.RequireDigit;
                options.Password.RequireLowercase = userPasswordConfig.RequireLowercase;
                options.Password.RequireNonAlphanumeric = userPasswordConfig.RequireNonAlphanumeric;
                options.Password.RequireUppercase = userPasswordConfig.RequireUppercase;
                options.Password.RequiredLength = userPasswordConfig.RequiredLength;
            })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        
        // Not sure if singelton works as it should here
        services.AddSingleton<IIdentityContextAccessor, IdentityContextAccessor>();
        services.AddSingleton<IIdentityContextDefault, IdentityContext>();

        var config = configuration.GetSection("Identity").Get<IdentityConfig>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = config!.ValidAudience,
                ValidIssuer = config.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret))
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Configuration.IdentityConstants.Policies.ExamplePolicy, policy =>
                policy.RequireClaim(Configuration.IdentityConstants.Claims.Scope, "default"));

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }


}
