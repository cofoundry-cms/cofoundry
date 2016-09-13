using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for encrypting/decrypting user passwords.
    /// </summary>
    public interface IPasswordCryptographyService
    {
        /// <summary>
        /// Verifies that an unecrypted password matches the specified hash.
        /// </summary>
        /// <param name="password">Plain text version of the password to check</param>
        /// <param name="hash">The encrypted hash to check the password against</param>
        /// <param name="version">The encryption version of the password hash.</param>
        bool Verify(string password, string hash, PasswordEncryptionVersion version);

        /// <summary>
        /// Creates a hash from the specified password string.
        /// </summary>
        /// <param name="password">Unencrypted password to hash.</param>
        PasswordCryptographyResult CreateHash(string password);
    }
}
