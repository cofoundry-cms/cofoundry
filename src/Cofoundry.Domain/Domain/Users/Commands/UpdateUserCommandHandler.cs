using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class UpdateUserCommandHandler
        : ICommandHandler<UpdateUserCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IUserContextCache _userContextCache;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;

        public UpdateUserCommandHandler(
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache,
            IUserUpdateCommandHelper userUpdateCommandHelper
            )
        {
            _dbContext = dbContext;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _permissionValidationService = permissionValidationService;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
            _userUpdateCommandHelper = userUpdateCommandHelper;
        }

        public async Task ExecuteAsync(UpdateUserCommand command, IExecutionContext executionContext)
        {
            var user = await _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            ValidatePermissions(userArea, executionContext);

            await UpdateRoleAsync(command, executionContext, user);
            await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);

            UpdateProperties(command, user);

            // Save
            await _dbContext.SaveChangesAsync();
            _transactionScopeManager.QueueCompletionTask(_dbContext, () => _userContextCache.Clear(user.UserId));
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

        private async Task UpdateRoleAsync(UpdateUserCommand command, IExecutionContext executionContext, User user)
        {
            // if a code is supplied we assume we're updating the role, otherwise check the id has changed
            if (!string.IsNullOrWhiteSpace(command.RoleCode)
                || (command.RoleId.HasValue && command.RoleId != user.RoleId))
            {
                var newRole = await _dbContext
                      .Roles
                      .FilterByIdOrCode(command.RoleId, command.RoleCode)
                      .SingleOrDefaultAsync();
                EntityNotFoundException.ThrowIfNull(newRole, command.RoleId?.ToString() ?? command.RoleCode);

                await _userCommandPermissionsHelper.ValidateNewRoleAsync(
                    newRole,
                    user.RoleId,
                    user.UserAreaCode,
                    executionContext
                    );

                user.Role = newRole;
            }
        }

        private static void UpdateProperties(UpdateUserCommand command, User user)
        {
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();
            user.RequirePasswordChange = command.RequirePasswordChange;
            user.IsEmailConfirmed = user.Email != null ? command.IsEmailConfirmed : false;
        }
    }
}
