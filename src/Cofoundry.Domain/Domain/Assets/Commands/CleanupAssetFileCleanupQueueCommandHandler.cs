using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Removes old completed entries from the AssetFileCleanupQueueItem
/// table.
/// </summary>
public class CleanupAssetFileCleanupQueueCommandHandler
    : ICommandHandler<CleanupAssetFileCleanupQueueCommand>
    , ICofoundryUserPermissionCheckHandler
{
    private readonly IAssetStoredProcedures _assetStoredProcedures;

    public CleanupAssetFileCleanupQueueCommandHandler(
        IAssetStoredProcedures assetStoredProcedures
        )
    {
        _assetStoredProcedures = assetStoredProcedures;
    }

    public Task ExecuteAsync(CleanupAssetFileCleanupQueueCommand command, IExecutionContext executionContext)
    {
        return _assetStoredProcedures.CleanupAssetFileCleanupQueueItemAsync(command.CompletedItemRetentionTime.TotalSeconds, command.DeadLetterRetentionTime.TotalSeconds);
    }
}
