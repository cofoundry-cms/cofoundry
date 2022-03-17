using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Deletes a block from a template region on a custom entity page.
/// </summary>
public class DeleteCustomEntityVersionPageBlockCommandHandler
    : ICommandHandler<DeleteCustomEntityVersionPageBlockCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public DeleteCustomEntityVersionPageBlockCommandHandler(
        CofoundryDbContext dbContext,
        ICustomEntityCache customEntityCache,
        IMessageAggregator messageAggregator,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _customEntityCache = customEntityCache;
        _messageAggregator = messageAggregator;
        _permissionValidationService = permissionValidationService;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(DeleteCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .CustomEntityVersionPageBlocks
            .Where(m => m.CustomEntityVersionPageBlockId == command.CustomEntityVersionPageBlockId)
            .Select(m => new
            {
                Block = m,
                CustomEntityId = m.CustomEntityVersion.CustomEntityId,
                CustomEntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                WorkFlowStatusId = m.CustomEntityVersion.WorkFlowStatusId
            })
            .SingleOrDefaultAsync();

        if (dbResult != null)
        {
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode, executionContext.UserContext);

            if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be deleted unless the entity is in draft status");
            }

            var customEntityVersionBlockId = dbResult.Block.CustomEntityVersionPageBlockId;
            _dbContext.CustomEntityVersionPageBlocks.Remove(dbResult.Block);

            await _dbContext.SaveChangesAsync();
            _transactionScopeFactory.QueueCompletionTask(_dbContext, () => OnTransactionComplete(dbResult.CustomEntityDefinitionCode, dbResult.CustomEntityId, customEntityVersionBlockId));

        }
    }

    private Task OnTransactionComplete(
        string customEntityDefinitionCode,
        int customEntityId,
        int customEntityVersionBlockId
        )
    {
        _customEntityCache.Clear(customEntityDefinitionCode, customEntityId);

        return _messageAggregator.PublishAsync(new CustomEntityVersionBlockDeletedMessage()
        {
            CustomEntityId = customEntityId,
            CustomEntityDefinitionCode = customEntityDefinitionCode,
            CustomEntityVersionId = customEntityVersionBlockId
        });
    }
}
