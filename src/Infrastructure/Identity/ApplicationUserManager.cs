using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Domain.Entities.User;
using FluentValidation.Results;
using Infrastructure.Identity.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity;

public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger,
        IUnitOfWork unitOfWork,
        ApplicationDbContext dbContext,
        IConfiguration configuration) : base(
        store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public Task<ApplicationUser?> GetAsync(int userId) => FindByIdAsync(userId.ToString());
    public async Task<ApplicationUser?> GetByUidAsync(Guid uid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Uid == uid);
    }
    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower(), cancellationToken);
    }
    public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
    {
        var result = await base.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync(new CancellationTokenSource().Token);

        return result;
    }

    public async Task ValidatePassword(ApplicationUser user, string password)
    {
        var passwordValidationResult = await new PasswordValidator<ApplicationUser>().ValidateAsync(this, user, password);

        if (!passwordValidationResult.Succeeded)
        {
            var validationErrors = new List<ValidationFailure>();

            foreach (var error in passwordValidationResult.Errors)
            {
                validationErrors.Add(new ValidationFailure(error.Code, error.Description));
            }
            throw new ValidationException(validationErrors);
        }
    }

    public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        await ValidatePassword(user, password);

        var result = await base.CreateAsync(user, password);

        await _unitOfWork.SaveChangesAsync(new CancellationTokenSource().Token);

        return result;
    }

    public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
    {
        var result = await base.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync(new CancellationTokenSource().Token);

        return result;
    }
    public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var claims = await base.GetClaimsAsync(user);
        claims.Add(new Claim(Configuration.IdentityConstants.Claims.Id, user.Id.ToString()));
        claims.Add(new Claim(Configuration.IdentityConstants.Claims.UserName, user.UserName!));
        claims.Add(new Claim(Configuration.IdentityConstants.Claims.FirstName, user.FirstName));
        claims.Add(new Claim(Configuration.IdentityConstants.Claims.LastName, user.LastName));

        return claims;
    }

    public string GenerateRandomPassword()
    {
        var userPasswordConfig = _configuration.GetSection(UserPasswordConfig.SectionName).Get<UserPasswordConfig>();

        var uppercaseLetters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        var lowercaseLetters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        var specialCharacters = new List<char> { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}' };
        var numbers = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        var passwordBuilder = new StringBuilder();

        if (userPasswordConfig!.RequireLowercase)
        {
            var randomLowercaseLetter = lowercaseLetters.OrderBy(s => Guid.NewGuid()).First();
            passwordBuilder.Append(randomLowercaseLetter);
        }

        if (userPasswordConfig!.RequireUppercase)
        {
            var randomUppercaseLetter = uppercaseLetters.OrderBy(s => Guid.NewGuid()).First();
            passwordBuilder.Append(randomUppercaseLetter);
        }

        if (userPasswordConfig!.RequireNonAlphanumeric)
        {
            var randomSpecialCharacter = specialCharacters.OrderBy(s => Guid.NewGuid()).First();
            passwordBuilder.Append(randomSpecialCharacter);
        }

        if (userPasswordConfig!.RequireDigit)
        {
            var randomNumber = numbers.OrderBy(s => Guid.NewGuid()).First();
            passwordBuilder.Append(randomNumber);
        }

        var requiredLength = userPasswordConfig!.RequiredLength * 2;
        var charactersToBeGeneratedCount = requiredLength - passwordBuilder.Length;

        for (var i = 1; i <= charactersToBeGeneratedCount; i++)
        {
            var characters = uppercaseLetters.Concat(lowercaseLetters)
                                             .Concat(specialCharacters)
                                             .Concat(numbers)
                                             .ToList();


            characters!.Shuffle();
            var randomCharacter = characters.OrderBy(s => Guid.NewGuid()).First();
            passwordBuilder.Append(randomCharacter);
        }

        var splittedPassword = passwordBuilder.ToString()
                                      .Split(',')
                                      .ToList();
        splittedPassword.Shuffle();

        var password = string.Join(',', splittedPassword);
        return password;
    }
}
