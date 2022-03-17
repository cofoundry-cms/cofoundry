using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// General task for cleaning up stale user data. Currently this only removes data 
/// from the <see cref="UserAuthenticationLog"/> and  <see cref="UserAuthenticationFailLog"/> tables.
/// </summary>
public class CleanupUsersCommandHandler
    : ICommandHandler<CleanupUsersCommand>
    , ICofoundryUserPermissionCheckHandler
{
    private readonly IUserStoredProcedures _userStoredProcedures;

    public CleanupUsersCommandHandler(
        IUserStoredProcedures userStoredProcedures
        )
    {
        _userStoredProcedures = userStoredProcedures;
    }

    public async Task ExecuteAsync(CleanupUsersCommand command, IExecutionContext executionContext)
    {
        var authenticationLogRetentionPeriodInSeconds = GetPeriodInSeconds(command, command.AuthenticationLogRetentionPeriod);
        var authenticationFailLogRetentionPeriodInSeconds = GetPeriodInSeconds(command, command.AuthenticationFailLogRetentionPeriod);

        if (authenticationLogRetentionPeriodInSeconds < 0 && authenticationFailLogRetentionPeriodInSeconds < 0) return;

        await _userStoredProcedures.CleanupAsync(command.UserAreaCode, authenticationLogRetentionPeriodInSeconds, authenticationFailLogRetentionPeriodInSeconds, executionContext.ExecutionDate);
    }

    private double GetPeriodInSeconds(CleanupUsersCommand command, TimeSpan? period)
    {
        if (!period.HasValue && !command.DefaultRetentionPeriod.HasValue) return 0;
        if (!period.HasValue) return command.DefaultRetentionPeriod.Value.TotalSeconds;

        return period.Value.TotalSeconds;
    }
}
