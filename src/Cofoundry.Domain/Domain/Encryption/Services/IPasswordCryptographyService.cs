using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for hashing and verifying user passwords.
    /// </summary>
    public interface IPasswordCryptographyService
    {
        /// <summary>
        /// Verifies that an unhashed password matches the specified hash.
        /// </summary>
        /// <param name="password">Plain text version of the password to check</param>
        /// <param name="hash">The hash to check the password against</param>
        /// <param name="hashVersion">The encryption version of the password hash.</param>
        bool Verify(string password, string hash, int hashVersion);

        /// <summary>
        /// Creates a hash from the specified password string.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        PasswordCryptographyResult CreateHash(string password);
    }
}
