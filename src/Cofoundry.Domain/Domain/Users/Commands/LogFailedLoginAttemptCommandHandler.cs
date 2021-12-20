using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class LogFailedLoginAttemptCommandHandler
        : ICommandHandler<LogFailedLoginAttemptCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly IMessageAggregator _messageAggregator;

        public LogFailedLoginAttemptCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IEntityFrameworkSqlExecutor sqlExecutor,
            IClientConnectionService clientConnectionService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _sqlExecutor = sqlExecutor;
            _clientConnectionService = clientConnectionService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(LogFailedLoginAttemptCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            await _sqlExecutor.ExecuteCommandAsync(_dbContext,
                "Cofoundry.FailedAuthticationAttempt_Add",
                new SqlParameter("UserAreaCode", command.UserAreaCode),
                new SqlParameter("Username", TextFormatter.Limit(command.Username, 150)),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );

            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(command));
        }

        private async Task OnTransactionComplete(LogFailedLoginAttemptCommand command)
        {
            await _messageAggregator.PublishAsync(new UserAuthenticationFailedMessage()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username
            });
        }
    }
}
