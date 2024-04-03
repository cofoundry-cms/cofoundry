using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Sets the status of a custom entity to un-published, but does not
/// remove the publish date, which is preserved so that it
/// can be used as a default when the user chooses to publish
/// again.
/// </summary>
public class UnPublishCustomEntityCommandHandler
    : ICommandHandler<UnPublishCustomEntityCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;

    public UnPublishCustomEntityCommandHandler(
        CofoundryDbContext dbContext,
        ICustomEntityCache customEntityCache,
        IMessageAggregator messageAggregator,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeManager,
        ICustomEntityStoredProcedures customEntityStoredProcedures
        )
    {
        _dbContext = dbContext;
        _customEntityCache = customEntityCache;
        _messageAggregator = messageAggregator;
        _permissionValidationService = permissionValidationService;
        _transactionScopeManager = transactionScopeManager;
        _customEntityStoredProcedures = customEntityStoredProcedures;
    }

    public async Task ExecuteAsync(UnPublishCustomEntityCommand command, IExecutionContext executionContext)
    {
        var customEntity = await _dbContext
            .CustomEntities
            .Where(p => p.CustomEntityId == command.CustomEntityId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(customEntity, command.CustomEntityId);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityPublishPermission>(customEntity.CustomEntityDefinitionCode, executionContext.UserContext);

        if (customEntity.PublishStatusCode == PublishStatusCode.Unpublished)
        {
            // No action
            return;
        }

        var version = await _dbContext
           .CustomEntityVersions
           .Include(v => v.CustomEntity)
           .Where(v => v.CustomEntityId == command.CustomEntityId && (v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published))
           .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
           .ThenByDescending(v => v.CreateDate)
           .FirstOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(version, command.CustomEntityId);

        customEntity.PublishStatusCode = PublishStatusCode.Unpublished;
        version.WorkFlowStatusId = (int)WorkFlowStatus.Draft;

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();
            await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.CustomEntityId);

            scope.QueueCompletionTask(() => OnTransactionComplete(customEntity));

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(CustomEntity entity)
    {
        _customEntityCache.Clear(entity.CustomEntityDefinitionCode, entity.CustomEntityId);

        return _messageAggregator.PublishAsync(new CustomEntityUnPublishedMessage()
        {
            CustomEntityId = entity.CustomEntityId,
            CustomEntityDefinitionCode = entity.CustomEntityDefinitionCode
        });
    }
}
