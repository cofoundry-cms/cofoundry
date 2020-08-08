using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Service for hashing and verifying user passwords. Handles multiple version 
    /// of the hashing function to maintain backwards compatibility.
    /// </summary>
    public class PasswordCryptographyService : IPasswordCryptographyService
    {
        /// <summary>
        /// Verifies that an unhashed password matches the specified hash.
        /// </summary>
        /// <param name="password">Plain text version of the password to check</param>
        /// <param name="hash">The hash to check the password against</param>
        /// <param name="hashVersion">The encryption version of the password hash.</param>
        public virtual bool Verify(string password, string hash, int hashVersion)
        {
            switch (hashVersion)
            {
                case (int)PasswordHashVersion.V1:
                    var service = new PasswordCryptographyV1();
                    return service.Verify(password, hash);
                case (int)PasswordHashVersion.V2:
                    return Defuse.PasswordCryptographyV2.VerifyPassword(password, hash);
                default:
                    throw new NotSupportedException("PasswordEncryptionVersion not recognised: " + hashVersion.ToString());
            }
        }

        /// <summary>
        /// Creates a hash from the specified password string.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        public virtual PasswordCryptographyResult CreateHash(string password)
        {
            var result = new PasswordCryptographyResult();
            result.Hash = Defuse.PasswordCryptographyV2.CreateHash(password);
            result.HashVersion = (int)PasswordHashVersion.V2;

            return result;
        }
    }
}
