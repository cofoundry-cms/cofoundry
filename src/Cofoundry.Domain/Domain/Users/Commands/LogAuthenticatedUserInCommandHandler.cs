using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class LogAuthenticatedUserInCommandHandler 
        : ICommandHandler<LogAuthenticatedUserInCommand>
        , IAsyncCommandHandler<LogAuthenticatedUserInCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly IClientConnectionService _clientConnectionService;

        public LogAuthenticatedUserInCommandHandler(
            CofoundryDbContext dbContext,
            IEntityFrameworkSqlExecutor sqlExecutor,
            IClientConnectionService clientConnectionService
            )
        {
            _dbContext = dbContext;
            _sqlExecutor = sqlExecutor;
            _clientConnectionService = clientConnectionService;
        }

        #endregion

        public void Execute(LogAuthenticatedUserInCommand command, IExecutionContext executionContext)
        {
            var user = Query(command.UserId).SingleOrDefault();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var connectionInfo = _clientConnectionService.GetConnectionInfo();
            SetLoggedIn(user, executionContext);
            _dbContext.SaveChanges();

            _sqlExecutor.ExecuteCommand("Cofoundry.UserLoginLog_Add",
                new SqlParameter("UserId", user.UserId),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }

        public async Task ExecuteAsync(LogAuthenticatedUserInCommand command, IExecutionContext executionContext)
        {
            var user = await Query(command.UserId).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var connectionInfo = _clientConnectionService.GetConnectionInfo();
            SetLoggedIn(user, executionContext);
            await _dbContext.SaveChangesAsync();

            await _sqlExecutor.ExecuteCommandAsync("Cofoundry.UserLoginLog_Add",
                new SqlParameter("UserId", user.UserId),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }

        private IQueryable<User> Query(int userId)
        {
            var user = _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn();

            return user;
        }

        private void SetLoggedIn(User user, IExecutionContext executionContext)
        {
            user.PreviousLoginDate = user.LastLoginDate;
            user.LastLoginDate = executionContext.ExecutionDate;
        }
    }
}
