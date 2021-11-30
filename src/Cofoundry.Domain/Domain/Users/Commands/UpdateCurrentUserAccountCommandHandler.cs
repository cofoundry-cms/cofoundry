using Cofoundry.Core;
using Cofoundry.Core.Validation;
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
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public UpdateCurrentUserAccountCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository,
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
            _emailAddressNormalizer = emailAddressNormalizer;
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

            Normalize(command);
            await UpdateEmailAsync(command, userId, user, executionContext);
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;

            await _dbContext.SaveChangesAsync();
        }

        private void Normalize(UpdateCurrentUserAccountCommand command)
        {
            command.Email = _emailAddressNormalizer.Normalize(command.Email);
            command.FirstName = command.FirstName?.Trim();
            command.LastName = command.LastName?.Trim();
        }

        private async Task UpdateEmailAsync(
            UpdateCurrentUserAccountCommand command,
            int userId,
            User user,
            IExecutionContext executionContext
            )
        {
            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);

            if (userArea.UseEmailAsUsername && user.Username != command.Email)
            {
                var uniqueQuery = new IsUsernameUniqueQuery()
                {
                    Username = command.Email,
                    UserId = userId,
                    UserAreaCode = userArea.UserAreaCode
                };

                if (!await _queryExecutor.ExecuteAsync(uniqueQuery, executionContext))
                {
                    throw ValidationErrorException.CreateWithProperties("This email is already registered", nameof(command.Email));
                }

                user.Username = command.Email;
            }

            user.Email = command.Email;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserAccountCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }
    }
}
