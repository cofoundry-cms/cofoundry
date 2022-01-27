using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Logs a user into the application for a specified user area
    /// using username and password credentials. Checks for valid
    /// credentials and includes additional security checking such
    /// as preventing excessive login attempts. Validation errors
    /// are thrown as ValidationExceptions.
    /// </summary>
    public class LogUserInWithCredentialsCommandHandler
        : ICommandHandler<LogUserInWithCredentialsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly ILogger<LogUserInWithCredentialsCommandHandler> _logger;
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly ILoginService _loginService;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

        public LogUserInWithCredentialsCommandHandler(
            ILogger<LogUserInWithCredentialsCommandHandler> logger,
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IUserStoredProcedures userStoredProcedures,
            ILoginService loginService,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper
            )
        {
            _logger = logger;
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _userStoredProcedures = userStoredProcedures;
            _loginService = loginService;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        }


        public async Task ExecuteAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext)) return;

            var authResult = await GetUserLoginInfoAsync(command, executionContext);
            authResult.ThrowIfNotSuccess();

            if (authResult.User.RequirePasswordChange)
            {
                // Even if a password change is required, we should take the oportunity to rehash
                if (authResult.User.PasswordRehashNeeded)
                {
                    await RehashPassword(authResult.User.UserId, command.Password);
                }

                throw new PasswordChangeRequiredException();
            }

            ValidateLoginArea(command.UserAreaCode, authResult.User.UserAreaCode);

            // Successful credentials auth invalidates any account recovery requests
            await _userStoredProcedures.InvalidateUserAccountRecoveryRequests(authResult.User.UserId, executionContext.ExecutionDate);

            await _loginService.LogAuthenticatedUserInAsync(
                command.UserAreaCode,
                authResult.User.UserId,
                command.RememberUser
                );
        }

        private static bool IsLoggedInAlready(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var currentContext = executionContext.UserContext;
            var isLoggedIntoDifferentUserArea = currentContext.UserArea?.UserAreaCode != command.UserAreaCode;

            return currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea;
        }

        private Task<UserLoginInfoAuthenticationResult> GetUserLoginInfoAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username,
                Password = command.Password,
                PropertyToValidate = nameof(command.Password)
            };

            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        private static void ValidateLoginArea(string userAreaToLogInto, string usersUserArea)
        {
            if (userAreaToLogInto != usersUserArea)
            {
                throw new ValidationErrorException("This user account is not permitted to log in via this route.");
            }
        }

        /// <remarks>
        /// So far this is only used here, but it could be separated into it's own
        /// command if it was used elsewhere. 
        /// </remarks>
        private async Task RehashPassword(int userId, string password)
        {
            var user = await _dbContext
                .Users
                .SingleOrDefaultAsync(u => u.UserId == userId);

            EntityNotFoundException.ThrowIfNull(user, userId);

            _logger.LogDebug("Rehashing password for user {UserId}", user.UserId);
            _passwordUpdateCommandHelper.UpdatePasswordHash(password, user);

            await _dbContext.SaveChangesAsync();
        }
    }
}
