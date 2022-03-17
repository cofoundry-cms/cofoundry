using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain;

/// <summary>
/// Service for hashing and verifying user passwords.
/// </summary>
/// <remarks>
/// Prior to v0.10 we've used a custom upgradeable algorithm for password
/// hashing (this area pre-dates .NET Core), but the default implementation 
/// now uses Microsoft.AspNetCore.Identity.IPasswordHasher. If you need to 
/// alter the hashing algorithm you can do so using any of the mechanisms 
/// or NuGet packages designed for IPasswordHasher.
/// </remarks>
public interface IPasswordCryptographyService
{
    /// <summary>
    /// Verifies that an unhashed password matches the specified hash.
    /// </summary>
    /// <param name="password">Plain text version of the password to check</param>
    /// <param name="hash">The hash to check the password against</param>
    /// <param name="hashVersion">The encryption version of the password hash.</param>
    /// <returns>
    /// A result that indicates if the password is valid and if it needs rehashing
    /// to a newer hash version.
    /// </returns>
    PasswordVerificationResult Verify(string password, string hash, int hashVersion);

    /// <summary>
    /// Creates a hash from the specified password string.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    PasswordCryptographyResult CreateHash(string password);
}
