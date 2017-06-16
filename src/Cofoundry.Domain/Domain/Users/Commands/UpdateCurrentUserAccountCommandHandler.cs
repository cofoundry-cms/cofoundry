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
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserAccountCommandHandler 
        : IAsyncCommandHandler<UpdateCurrentUserAccountCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserAccountCommand>
    {
        #region consructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaRepository _userAreaRepository;

        public UpdateCurrentUserAccountCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserAreaRepository userAreaRepository
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

            await UpdateEmail(command, userId, user);

            user.FirstName = command.FirstName.Trim();
            user.LastName = command.LastName.Trim();

            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateEmail(UpdateCurrentUserAccountCommand command, int userId, User user)
        {
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            if (userArea.UseEmailAsUsername && user.Username != command.Email)
            {
                var uniqueQuery = new IsUsernameUniqueQuery()
                {
                    Username = command.Email,
                    UserId = userId,
                    UserAreaCode = CofoundryAdminUserArea.AreaCode
                };

                if (!await _queryExecutor.ExecuteAsync(uniqueQuery))
                {
                    throw new PropertyValidationException("This email is already registered", "Email");
                }

                user.Username = command.Email;
            }

            user.Email = command.Email;
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
