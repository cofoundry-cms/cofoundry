using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates an existing block within a template region 
/// of a custom entity page.
/// </summary>
public class UpdateCustomEntityVersionPageBlockCommandHandler
    : ICommandHandler<UpdateCustomEntityVersionPageBlockCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICustomEntityCache _customEntityCache;
    private readonly IPageBlockCommandHelper _pageBlockCommandHelper;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public UpdateCustomEntityVersionPageBlockCommandHandler(
        CofoundryDbContext dbContext,
        ICommandExecutor commandExecutor,
        ICustomEntityCache pageCache,
        IPageBlockCommandHelper pageBlockCommandHelper,
        IMessageAggregator messageAggregator,
        IPermissionValidationService permissionValidationService,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _commandExecutor = commandExecutor;
        _customEntityCache = pageCache;
        _pageBlockCommandHelper = pageBlockCommandHelper;
        _messageAggregator = messageAggregator;
        _permissionValidationService = permissionValidationService;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(UpdateCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
    {
        var dbBlock = await _dbContext
            .CustomEntityVersionPageBlocks
            .Include(m => m.PageBlockTypeTemplate)
            .Include(m => m.PageBlockType)
            .Include(m => m.CustomEntityVersion)
            .ThenInclude(v => v.CustomEntity)
            .Where(l => l.CustomEntityVersionPageBlockId == command.CustomEntityVersionPageBlockId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(dbBlock, command.CustomEntityVersionPageBlockId);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

        if (dbBlock.CustomEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
        {
            throw new NotPermittedException("Page blocks cannot be updated unless the entity is in draft status");
        }

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _pageBlockCommandHelper.UpdateModelAsync(command, dbBlock);
            await _dbContext.SaveChangesAsync();

            var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                CustomEntityVersionPageBlockEntityDefinition.DefinitionCode,
                dbBlock.CustomEntityVersionPageBlockId,
                command.DataModel);

            await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);

            scope.QueueCompletionTask(() => OnTransactionComplete(dbBlock));

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(CustomEntityVersionPageBlock dbBlock)
    {
        _customEntityCache.Clear(dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode, dbBlock.CustomEntityVersion.CustomEntity.CustomEntityId);

        return _messageAggregator.PublishAsync(new CustomEntityVersionBlockUpdatedMessage()
        {
            CustomEntityId = dbBlock.CustomEntityVersion.CustomEntityId,
            CustomEntityDefinitionCode = dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
            CustomEntityVersionBlockId = dbBlock.CustomEntityVersionPageBlockId
        });
    }
}
