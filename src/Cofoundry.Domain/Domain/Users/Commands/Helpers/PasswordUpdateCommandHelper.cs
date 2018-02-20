using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by in password update commands for shared functionality.
    /// </summary>
    public class PasswordUpdateCommandHelper : IPasswordUpdateCommandHelper
    {
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordCryptographyService _passwordCryptographyService;

        public PasswordUpdateCommandHelper(
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordCryptographyService passwordCryptographyService
            )
        {
            _passwordCryptographyService = passwordCryptographyService;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
        }

        public void ValidateUserArea(IUserAreaDefinition userArea)
        {
            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("Cannot update the password to account in a user area that does not allow password logins.");
            }
        }

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }

        public void UpdatePassword(string newPassword, User user, IExecutionContext executionContext)
        {
            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            var hashResult = _passwordCryptographyService.CreateHash(newPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
        }
    }
}
