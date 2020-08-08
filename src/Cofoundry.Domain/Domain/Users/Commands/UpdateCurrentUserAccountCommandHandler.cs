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
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserAccountCommandHandler 
        : ICommandHandler<UpdateCurrentUserAccountCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserAccountCommand>
    {
        #region consructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public UpdateCurrentUserAccountCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateCurrentUserAccountCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);
            var userId = executionContext.UserContext.UserId.Value;

            var user = await _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(userId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, userId);

            await UpdateEmailAsync(command, userId, user, executionContext);

            user.FirstName = command.FirstName.Trim();
            user.LastName = command.LastName.Trim();

            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateEmailAsync(
            UpdateCurrentUserAccountCommand command, 
            int userId, 
            User user,
            IExecutionContext executionContext
            )
        {
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            var newEmail = command.Email?.Trim();

            if (userArea.UseEmailAsUsername && user.Username != newEmail)
            {
                var uniqueQuery = new IsUsernameUniqueQuery()
                {
                    Username = newEmail,
                    UserId = userId,
                    UserAreaCode = CofoundryAdminUserArea.AreaCode
                };

                if (!await _queryExecutor.ExecuteAsync(uniqueQuery, executionContext))
                {
                    throw ValidationErrorException.CreateWithProperties("This email is already registered", "Email");
                }

                user.Username = newEmail;
            }

            user.Email = newEmail;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserAccountCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }

        #endregion
    }
}
