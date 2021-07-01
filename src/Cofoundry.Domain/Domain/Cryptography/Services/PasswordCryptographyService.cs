using Cofoundry.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPasswordHasher<PasswordHasherUser> _passwordHasher;

        public PasswordCryptographyService(
            IPasswordHasher<PasswordHasherUser> passwordHasher
            )
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Verifies that an unhashed password matches the specified hash.
        /// </summary>
        /// <param name="password">Plain text version of the password to check</param>
        /// <param name="hash">The hash to check the password against</param>
        /// <param name="hashVersion">The encryption version of the password hash.</param>
        public virtual PasswordVerificationResult Verify(string password, string hash, int hashVersion)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentEmptyException(nameof(hash));

            if (string.IsNullOrWhiteSpace(password)) return PasswordVerificationResult.Failed;

            switch (hashVersion)
            {
                case (int)PasswordHashVersion.V1:
                    var isV1Valid = new PasswordCryptographyV1().Verify(password, hash);
                    return FormatOldPasswordVersionResult(isV1Valid);
                case (int)PasswordHashVersion.V2:
                    var isV2Valid = Defuse.PasswordCryptographyV2.VerifyPassword(password, hash);
                    return FormatOldPasswordVersionResult(isV2Valid);
                case (int)PasswordHashVersion.V3:
                    var v3Result = _passwordHasher.VerifyHashedPassword(new PasswordHasherUser(), hash, password);
                    return v3Result;
                default:
                    throw new NotSupportedException("PasswordEncryptionVersion not recognised: " + hashVersion.ToString());
            }
        }

        private PasswordVerificationResult FormatOldPasswordVersionResult(bool isValid)
        {
            return isValid ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Creates a hash from the specified password string.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        public virtual PasswordCryptographyResult CreateHash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentEmptyException(nameof(password));

            var result = new PasswordCryptographyResult();
            result.Hash = _passwordHasher.HashPassword(new PasswordHasherUser(), password);
            result.HashVersion = (int)PasswordHashVersion.V3;

            return result;
        }
    }
}
