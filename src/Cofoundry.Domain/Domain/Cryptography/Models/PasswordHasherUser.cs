namespace Cofoundry.Domain.Cryptography;

/// <summary>
/// Dummy user class to provide to the Microsoft.AspNetCore.Identity.IPasswordHasher 
/// used by the PasswordCryptographyService. There's no implementation as it's not 
/// required by the default hasher.
/// </summary>
public class PasswordHasherUser
{
}
