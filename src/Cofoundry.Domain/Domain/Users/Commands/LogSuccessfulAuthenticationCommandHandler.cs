using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Logs user auditing information in the database to record 
    /// the successful authentication of a user account.
    /// </summary>
    public class LogSuccessfulAuthenticationCommandHandler
        : ICommandHandler<LogSuccessfulAuthenticationCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly IClientConnectionService _clientConnectionService;

        public LogSuccessfulAuthenticationCommandHandler(
            CofoundryDbContext dbContext,
            IEntityFrameworkSqlExecutor sqlExecutor,
            IClientConnectionService clientConnectionService
            )
        {
            _dbContext = dbContext;
            _sqlExecutor = sqlExecutor;
            _clientConnectionService = clientConnectionService;
        }

        public async Task ExecuteAsync(LogSuccessfulAuthenticationCommand command, IExecutionContext executionContext)
        {
            var user = await QueryUserAsync(command.UserId);
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            SetLoggedIn(user, executionContext);
            await _dbContext.SaveChangesAsync();

            await _sqlExecutor.ExecuteCommandAsync(_dbContext,
                "Cofoundry.UserLoginLog_Add",
                new SqlParameter("UserId", user.UserId),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }

        private async Task<User> QueryUserAsync(int userId)
        {
            var user = await _dbContext
                .Users
                .FilterById(userId)
                .FilterCanSignIn()
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, userId);

            return user;
        }

        private void SetLoggedIn(User user, IExecutionContext executionContext)
        {
            user.PreviousSignInDate = user.LastSignInDate;
            user.LastSignInDate = executionContext.ExecutionDate;
        }
    }
}
