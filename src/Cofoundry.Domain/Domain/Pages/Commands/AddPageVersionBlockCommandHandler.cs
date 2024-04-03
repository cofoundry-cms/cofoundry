using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class AddPageVersionBlockCommandHandler
    : ICommandHandler<AddPageVersionBlockCommand>
    , IPermissionRestrictedCommandHandler<AddPageVersionBlockCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly EntityOrderableHelper _entityOrderableHelper;
    private readonly IPageCache _pageCache;
    private readonly IPageBlockCommandHelper _pageBlockCommandHelper;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public AddPageVersionBlockCommandHandler(
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        EntityOrderableHelper entityOrderableHelper,
        IPageCache pageCache,
        IPageBlockCommandHelper pageBlockCommandHelper,
        ICommandExecutor commandExecutor,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
        _entityOrderableHelper = entityOrderableHelper;
        _pageCache = pageCache;
        _pageBlockCommandHelper = pageBlockCommandHelper;
        _commandExecutor = commandExecutor;
        _messageAggregator = messageAggregator;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(AddPageVersionBlockCommand command, IExecutionContext executionContext)
    {
        var templateRegion = await _dbContext
            .PageTemplateRegions
            .FirstOrDefaultAsync(l => l.PageTemplateRegionId == command.PageTemplateRegionId);
        EntityNotFoundException.ThrowIfNull(templateRegion, command.PageTemplateRegionId);

        var pageVersion = _dbContext
            .PageVersions
            .Include(s => s.PageVersionBlocks)
            .FirstOrDefault(v => v.PageVersionId == command.PageVersionId);
        EntityNotFoundException.ThrowIfNull(pageVersion, command.PageVersionId);

        if (pageVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
        {
            throw new NotPermittedException("Page blocks cannot be added unless the page version is in draft status");
        }

        if (pageVersion.PageTemplateId != templateRegion.PageTemplateId)
        {
            throw ValidationErrorException.CreateWithProperties("This region does not belong to the template associated with the page.", nameof(command.PageTemplateRegionId));
        }

        var pageVersionBlocks = pageVersion
            .PageVersionBlocks
            .Where(m => m.PageTemplateRegionId == templateRegion.PageTemplateRegionId);

        PageVersionBlock? adjacentItem = null;
        if (command.AdjacentVersionBlockId.HasValue)
        {
            adjacentItem = pageVersionBlocks
                .SingleOrDefault(m => m.PageVersionBlockId == command.AdjacentVersionBlockId);
            EntityNotFoundException.ThrowIfNull(adjacentItem, command.AdjacentVersionBlockId);
        }

        var newBlock = new PageVersionBlock();
        newBlock.PageTemplateRegion = templateRegion;

        await _pageBlockCommandHelper.UpdateModelAsync(command, newBlock);

        newBlock.PageVersion = pageVersion;
        newBlock.UpdateDate = executionContext.ExecutionDate;

        _entityAuditHelper.SetCreated(newBlock, executionContext);
        _entityOrderableHelper.SetOrderingForInsert(pageVersionBlocks, newBlock, command.InsertMode, adjacentItem);

        _dbContext.PageVersionBlocks.Add(newBlock);
        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            await _dbContext.SaveChangesAsync();

            var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                PageVersionBlockEntityDefinition.DefinitionCode,
                newBlock.PageVersionBlockId,
                command.DataModel);

            await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);

            scope.QueueCompletionTask(() => OnTransactionComplete(pageVersion, newBlock));

            await scope.CompleteAsync();
        }

        command.OutputPageBlockId = newBlock.PageVersionBlockId;
    }

    private Task OnTransactionComplete(PageVersion pageVersion, PageVersionBlock newBlock)
    {
        _pageCache.Clear(pageVersion.PageId);

        return _messageAggregator.PublishAsync(new PageVersionBlockAddedMessage()
        {
            PageId = pageVersion.PageId,
            PageVersionBlockId = newBlock.PageVersionBlockId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddPageVersionBlockCommand command)
    {
        yield return new PageUpdatePermission();
    }
}
