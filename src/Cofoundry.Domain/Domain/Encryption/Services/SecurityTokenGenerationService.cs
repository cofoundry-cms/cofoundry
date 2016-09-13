using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SecurityTokenGenerationService : ISecurityTokenGenerationService
    {
        private IPasswordGenerationService _passwordGenerationService;
        
        public SecurityTokenGenerationService(
            IPasswordGenerationService passwordGenerationService
            )
        {
            _passwordGenerationService = passwordGenerationService;
        }

        /// <summary>
        /// Generates a unique and random security token that can be used to verify
        /// a request without a username and password, e.g. for a password reset link
        /// </summary>
        public string Generate()
        {
            // Get a little uniquess
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
            // Sprinkle some random padding
            var randomString = _passwordGenerationService.Generate(20);
            // Encrypt the lot to obfuscate it
            var token = Defuse.PasswordCryptographyV2.CreateHash(guid + randomString);

            return token;
        }
    }
}
