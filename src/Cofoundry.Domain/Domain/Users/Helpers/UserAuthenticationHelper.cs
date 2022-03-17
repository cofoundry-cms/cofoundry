using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Shared user authentication methods
/// </summary>
public class UserAuthenticationHelper
{
    private readonly IPasswordCryptographyService _cryptographyService;
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public UserAuthenticationHelper(
        IPasswordCryptographyService cryptographyService,
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _cryptographyService = cryptographyService;
        _userAreaRepository = userAreaRepository;
    }

    public PasswordVerificationResult VerifyPassword(User user, string password)
    {
        if (user == null) return PasswordVerificationResult.Failed;

        var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);

        if (!userArea.AllowPasswordSignIn)
        {
            throw new InvalidOperationException("This user is not permitted to log in with a password.");
        }

        if (String.IsNullOrWhiteSpace(user.Password) || !user.PasswordHashVersion.HasValue)
        {
            throw new InvalidOperationException("Cannot authenticate via password because the specified account does not have a password set.");
        }

        var result = _cryptographyService.Verify(password, user.Password, user.PasswordHashVersion.Value);

        return result;
    }
}
