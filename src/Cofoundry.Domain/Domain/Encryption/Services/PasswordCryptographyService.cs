using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for encrypting/decrypting user passwords. Handles multiple version 
    /// of the encryption to maintain backwards compatibility.
    /// </summary>
    public class PasswordCryptographyService : IPasswordCryptographyService
    {
        /// <summary>
        /// Verifies that an unecrypted password matches the specified hash.
        /// </summary>
        /// <param name="password">Plain text version of the password to check</param>
        /// <param name="hash">The encrypted hash to check the password against</param>
        /// <param name="version">The encryption version of the password hash.</param>
        public bool Verify(string password, string hash, PasswordEncryptionVersion version)
        {
            switch (version)
            {
                case PasswordEncryptionVersion.V1:
                    var service = new PasswordCryptographyV1();
                    return service.Verify(password, hash);
                case PasswordEncryptionVersion.V2:
                    return Defuse.PasswordCryptographyV2.VerifyPassword(password, hash);
                default:
                    throw new ApplicationException("PasswordEncryptionVersion not recognised: " + version.ToString());
            }
        }

        /// <summary>
        /// Creates a hash from the specified password string.
        /// </summary>
        /// <param name="password">Unencrypted password to hash.</param>
        public PasswordCryptographyResult CreateHash(string password)
        {
            var result = new PasswordCryptographyResult();
            result.Hash = Defuse.PasswordCryptographyV2.CreateHash(password);
            result.EncryptionVersion = PasswordEncryptionVersion.V2;

            return result;
        }
    }
}
