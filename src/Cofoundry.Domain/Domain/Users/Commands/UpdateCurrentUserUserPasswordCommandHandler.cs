using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// OldPassword field to authenticate the request.
    /// </summary>
    public class UpdateCurrentUserUserPasswordCommandHandler 
        : IAsyncCommandHandler<UpdateCurrentUserUserPasswordCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserUserPasswordCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaRepository _userAreaRepository;

        public UpdateCurrentUserUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            IPasswordCryptographyService passwordCryptographyService,
            UserAuthenticationHelper userAuthenticationHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaRepository userAreaRepository
            )
        {
            _dbContext = dbContext;
            _passwordCryptographyService = passwordCryptographyService;
            _userAuthenticationHelper = userAuthenticationHelper;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region execution

        public void Execute(UpdateCurrentUserUserPasswordCommand command, IExecutionContext executionContext)
        {
            var user = QueryUser(command, executionContext).SingleOrDefault();
            UpdatePassword(command, executionContext, user);
            _dbContext.SaveChanges();
        }

        public async Task ExecuteAsync(UpdateCurrentUserUserPasswordCommand command, IExecutionContext executionContext)
        {
            var user = await QueryUser(command, executionContext).SingleOrDefaultAsync();
            UpdatePassword(command, executionContext, user);
            await _dbContext.SaveChangesAsync();
        }

        private void UpdatePassword(UpdateCurrentUserUserPasswordCommand command, IExecutionContext executionContext, User user)
        {
            EntityNotFoundException.ThrowIfNull(user, executionContext.UserContext.UserId);
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("Cannot update the password to account in a user area that does not allow password logins.");
            }

            if (!_userAuthenticationHelper.IsPasswordCorrect(user, command.OldPassword))
            {
                throw new PropertyValidationException("Incorrect password", "OldPassword");
            }

            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            var hashResult = _passwordCryptographyService.CreateHash(command.NewPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
        }

        private IQueryable<User> QueryUser(UpdateCurrentUserUserPasswordCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);

            var user = _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(executionContext.UserContext.UserId.Value);

            return user;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserUserPasswordCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }

        #endregion
    }
}
