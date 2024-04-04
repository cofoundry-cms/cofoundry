namespace Cofoundry.Domain;

/// <summary>
/// Enum used to indicate the version of the function used to hash passwords. 
/// Currently <see cref="V3"/> is used to defer the implementation to 
/// <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/> and so it is unlikely
/// there will be any more official versions, however this remains in place for
/// backwards compatibility and to allow for custom implementations.
/// </summary>
public enum PasswordHashVersion
{
    /// <summary>
    /// Original version (SHA1, 32-bit salt).
    /// </summary>
    /// <remarks>
    /// This has never been used in Cofoundry and is only here
    /// for compatibility with very very (probably non-existent) 
    /// systems implemented with a prior product.
    /// </remarks>
    V1 = 1,
    /// <summary>
    /// Version used in Cofoundry prior to v0.10. PBKDF2 with HMAC-SHA1, 192-bit salt, 64000 iterations
    /// </summary>
    V2 = 2,
    /// <summary>
    /// Uses <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/> to defer the algorithm 
    /// choice to the one that is implemented by the .NET Core security team. 
    /// As of v3 uses "PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations".
    /// </summary>
    V3 = 3
}
