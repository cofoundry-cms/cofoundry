using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Writes a log entry to the FailedAuthenticationAttempt table indicating
/// an unsuccessful login attempt occurred.
/// </summary>
public class LogFailedAuthenticationAttemptCommandHandler
    : ICommandHandler<LogFailedAuthenticationAttemptCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IDomainRepository _domainRepository;
    private readonly IUserStoredProcedures _userStoredProcedures;
    private readonly IClientConnectionService _clientConnectionService;
    private readonly IMessageAggregator _messageAggregator;

    public LogFailedAuthenticationAttemptCommandHandler(
        IDomainRepository domainRepository,
        IUserStoredProcedures userStoredProcedures,
        IClientConnectionService clientConnectionService,
        IMessageAggregator messageAggregator
        )
    {
        _domainRepository = domainRepository;
        _userStoredProcedures = userStoredProcedures;
        _clientConnectionService = clientConnectionService;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(LogFailedAuthenticationAttemptCommand command, IExecutionContext executionContext)
    {
        var connectionInfo = _clientConnectionService.GetConnectionInfo();

        await _userStoredProcedures.LogAuthenticationFailedAsync(
            command.UserAreaCode,
            TextFormatter.Limit(command.Username, 150),
            connectionInfo.IPAddress,
            executionContext.ExecutionDate
            );

        await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(command));
    }

    private async Task OnTransactionComplete(LogFailedAuthenticationAttemptCommand command)
    {
        await _messageAggregator.PublishAsync(new UserAuthenticationFailedMessage()
        {
            UserAreaCode = command.UserAreaCode,
            Username = command.Username
        });
    }
}
