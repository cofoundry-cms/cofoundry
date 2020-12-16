using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// OldPassword field to authenticate the request.
    /// </summary>
    public class UpdateCurrentUserPasswordCommandHandler 
        : ICommandHandler<UpdateCurrentUserPasswordCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserPasswordCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

        public UpdateCurrentUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper
            )
        {
            _dbContext = dbContext;
            _userAuthenticationHelper = userAuthenticationHelper;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        }

        #endregion

        #region execution
        
        public async Task ExecuteAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);

            var user = await GetUser(command, executionContext);
            UpdatePassword(command, executionContext, user);
            await _dbContext.SaveChangesAsync();
        }

        private void UpdatePassword(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext, User user)
        {
            EntityNotFoundException.ThrowIfNull(user, executionContext.UserContext.UserId);
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);

            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            if (!_userAuthenticationHelper.IsPasswordCorrect(user, command.OldPassword))
            {
                throw ValidationErrorException.CreateWithProperties("Incorrect password", "OldPassword");
            }

            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
        }

        private Task<User> GetUser(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
        {
            return _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(executionContext.UserContext.UserId.Value)
                .SingleOrDefaultAsync();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserPasswordCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }

        #endregion
    }
}
