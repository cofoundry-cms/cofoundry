using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data.Internal;
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
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IClientConnectionService _clientConnectionService;

        public LogSuccessfulAuthenticationCommandHandler(
            IUserStoredProcedures userStoredProcedures,
            IClientConnectionService clientConnectionService
            )
        {
            _userStoredProcedures = userStoredProcedures;
            _clientConnectionService = clientConnectionService;
        }

        public async Task ExecuteAsync(LogSuccessfulAuthenticationCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            await _userStoredProcedures.LogAuthenticationSuccess(
                command.UserId,
                connectionInfo.IPAddress,
                executionContext.ExecutionDate
                );
        }
    }
}
