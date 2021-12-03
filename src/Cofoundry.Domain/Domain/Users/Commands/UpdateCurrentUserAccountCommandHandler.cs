using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserAccountCommandHandler
        : ICommandHandler<UpdateCurrentUserAccountCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserAccountCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UpdateCurrentUserAccountCommandHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

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

            await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();

            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserAccountCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }
    }
}
