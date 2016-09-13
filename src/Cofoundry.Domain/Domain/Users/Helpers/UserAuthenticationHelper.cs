using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Shared auser uthentication methods
    /// </summary>
    public class UserAuthenticationHelper
    {
        private readonly IPasswordCryptographyService _cryptographyService;
        private readonly IUserAreaRepository _userAreaRepository;

        public UserAuthenticationHelper(
            IPasswordCryptographyService cryptographyService,
            IUserAreaRepository userAreaRepository
            )
        {
            _cryptographyService = cryptographyService;
            _userAreaRepository = userAreaRepository;
        }

        public bool IsPasswordCorrect(User user, string password)
        {
            if (user == null) return false;

            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("This user is not permitted to log in with a password.");
            }

            if (String.IsNullOrWhiteSpace(user.Password) || !user.PasswordEncryptionVersion.HasValue)
            {
                throw new InvalidOperationException("Cannot authenticate via password because the specified account does not have a password set.");
            }

            bool result = _cryptographyService.Verify(password, user.Password, (PasswordEncryptionVersion)user.PasswordEncryptionVersion.Value);

            return result;
        }
    }
}
