namespace Cofoundry.Domain;

/// <summary>
/// The result of a cryptographic hash operation on a 
/// password.
/// </summary>
public class PasswordCryptographyResult
{
    /// <summary>
    /// An identifier representing the cryptographic hash function
    /// used to create the hash. This allows for upgradeable hash functions.
    /// This value is typically represented by <see cref="PasswordHashVersion"/>
    /// but custom values can be used for custom hashing implementations.
    /// </summary>
    public required int HashVersion { get; set; }

    /// <summary>
    /// A string representaion of the hashed password.
    /// </summary>
    public required string Hash { get; set; }
}
