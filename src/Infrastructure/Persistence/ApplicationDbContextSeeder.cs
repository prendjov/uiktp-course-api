using Application.Common.Authorizaion;
using Domain.Entities.Medias;
using Domain.Entities.User;
using DTO.Enums.Media;
using DTO.Enums.User;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task<ApplicationUser> SeedDefaultRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        var superAdminRole = new IdentityRole<int>(Role.SuperAdmin);
        var professorRole = new IdentityRole<int>(Role.Professor);
        var professorHelperRole = new IdentityRole<int>(Role.ProfessorHelper);
        var studentRole = new IdentityRole<int>(Role.Student);

        // Create the default roles if not exsist
        if (roleManager.Roles.All(r => r.Name != superAdminRole.Name)) await roleManager.CreateAsync(superAdminRole);
        if (roleManager.Roles.All(r => r.Name != professorRole.Name)) await roleManager.CreateAsync(professorRole);
        if (roleManager.Roles.All(r => r.Name != professorHelperRole.Name)) await roleManager.CreateAsync(professorHelperRole);
        if (roleManager.Roles.All(r => r.Name != studentRole.Name)) await roleManager.CreateAsync(studentRole);

        var administrator = new ApplicationUser
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "administrator@localhost",
            Email = "administrator@localhost",
            EmailConfirmed = true,
            Media = new Media(MediaEntityType.User),
            Status = UserStatus.Active
        };

        // Check if default administrator already created
        var existedAdministrator = userManager.Users.FirstOrDefault(u => u.UserName == administrator.UserName);
        if (existedAdministrator != null) return existedAdministrator;

        // Create default administraotor
        await userManager.CreateAsync(administrator, "Administrator1!");
        await userManager.AddClaimAsync(administrator, new Claim("scope", "default"));

        // Add the user to the role (Administrator)
        await userManager.AddToRolesAsync(
            administrator,
            new[] { superAdminRole.Name! });

        return administrator;
    }
}

