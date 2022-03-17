using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Marks an authorized task as complete. The command does not validate 
/// the authorization task or token, which is expected to be done prior 
/// to invoking this command. To validate an auhtorized task token use
/// <see cref="ValidateAuthorizedTaskTokenQuery"/>.
/// </summary>
public class InvalidateAuthorizedTaskBatchCommandHandler
    : ICommandHandler<InvalidateAuthorizedTaskBatchCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IAuthorizedTaskStoredProcedures _authorizedTaskStoredProcedures;

    public InvalidateAuthorizedTaskBatchCommandHandler(
        IAuthorizedTaskStoredProcedures authorizedTaskStoredProcedures
        )
    {
        _authorizedTaskStoredProcedures = authorizedTaskStoredProcedures;
    }

    public async Task ExecuteAsync(InvalidateAuthorizedTaskBatchCommand command, IExecutionContext executionContext)
    {
        var authenticationCodes = command.AuthorizedTaskTypeCodes?.ToArray();
        await _authorizedTaskStoredProcedures.InvalidateUserAccountRecoveryRequestsAsync(command.UserId, authenticationCodes, executionContext.ExecutionDate);
    }
}
