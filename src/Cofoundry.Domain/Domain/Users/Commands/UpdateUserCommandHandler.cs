using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
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
        private readonly IQueryExecutor _queryExecutor;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IUserContextCache _userContextCache;
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public UpdateUserCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache,
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _permissionValidationService = permissionValidationService;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public async Task ExecuteAsync(UpdateUserCommand command, IExecutionContext executionContext)
        {
            Normalize(command);

            // Get User
            var user = await _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            // Validate
            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            ValidatePermissions(userArea, executionContext);
            ValidateCommand(command, userArea);
            await ValidateIsUniqueAsync(command, userArea, executionContext);

            // Map updates
            await UpdateRoleAsync(command, executionContext, user);
            UpdateProperties(command, user, userArea);

            // Save
            await _dbContext.SaveChangesAsync();
            _transactionScopeManager.QueueCompletionTask(_dbContext, () => _userContextCache.Clear(user.UserId));
        }

        private void Normalize(UpdateUserCommand command)
        {
            command.FirstName = command.FirstName?.Trim();
            command.LastName = command.LastName?.Trim();
            command.Email = _emailAddressNormalizer.Normalize(command.Email);
            command.Username = command.Username?.Trim();
        }

        private void ValidateCommand(UpdateUserCommand command, IUserAreaDefinition userArea)
        {
            if (userArea.UseEmailAsUsername && string.IsNullOrEmpty(command.Email))
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", nameof(command.Email));
            }

            if (!userArea.UseEmailAsUsername && string.IsNullOrWhiteSpace(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field is required", nameof(command.Username));
            }
        }

        private async Task ValidateIsUniqueAsync(
            UpdateUserCommand command,
            IUserAreaDefinition userArea,
            IExecutionContext executionContext
            )
        {
            var query = new IsUsernameUniqueQuery()
            {
                UserId = command.UserId,
                UserAreaCode = userArea.UserAreaCode
            };

            if (userArea.UseEmailAsUsername)
            {
                query.Username = command.Email;
            }
            else
            {
                query.Username = command.Username;
            }

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                if (userArea.UseEmailAsUsername)
                {
                    throw ValidationErrorException.CreateWithProperties("This email is already registered", nameof(command.Email));
                }
                else
                {
                    throw ValidationErrorException.CreateWithProperties("This username is already registered", nameof(command.Username));
                }
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

        private static void UpdateProperties(UpdateUserCommand command, User user, IUserAreaDefinition userArea)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.Email = command.Email;

            if (userArea.UseEmailAsUsername)
            {
                user.Username = command.Email;
            }
            else
            {
                user.Username = command.Username;
            }

            user.RequirePasswordChange = command.RequirePasswordChange;
            user.IsEmailConfirmed = command.IsEmailConfirmed;
        }
    }
}
