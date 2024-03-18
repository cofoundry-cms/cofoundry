﻿using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class MovePageVersionBlockCommandHandler
    : ICommandHandler<MovePageVersionBlockCommand>
    , IPermissionRestrictedCommandHandler<MovePageVersionBlockCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public MovePageVersionBlockCommandHandler(
        CofoundryDbContext dbContext,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(MovePageVersionBlockCommand command, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .PageVersionBlocks
            .Where(b => b.PageVersionBlockId == command.PageVersionBlockId)
            .Select(b => new
            {
                Block = b,
                b.PageVersion.PageId,
                b.PageVersion.WorkFlowStatusId
            })
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(dbResult, command.PageVersionBlockId);

        if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
        {
            throw new NotPermittedException("Page blocks cannot be moved unless the page version is in draft status");
        }

        var block = dbResult.Block;
        var blockToSwapWithQuery = _dbContext
            .PageVersionBlocks
            .Where(p => p.PageTemplateRegionId == block.PageTemplateRegionId && p.PageVersionId == block.PageVersionId);

        PageVersionBlock? blockToSwapWith;

        switch (command.Direction)
        {
            case OrderedItemMoveDirection.Up:
                blockToSwapWith = await blockToSwapWithQuery
                    .Where(p => p.Ordering < block.Ordering)
                    .OrderByDescending(p => p.Ordering)
                    .FirstOrDefaultAsync();
                break;
            case OrderedItemMoveDirection.Down:
                blockToSwapWith = await blockToSwapWithQuery
                    .Where(p => p.Ordering > block.Ordering)
                    .OrderBy(p => p.Ordering)
                    .FirstOrDefaultAsync();
                break;
            default:
                throw new InvalidOperationException("OrderedItemMoveDirection not recognized: " + command.Direction);
        }

        if (blockToSwapWith == null)
        {
            return;
        }

        int oldOrdering = block.Ordering;
        block.Ordering = blockToSwapWith.Ordering;
        block.UpdateDate = executionContext.ExecutionDate;
        blockToSwapWith.Ordering = oldOrdering;
        blockToSwapWith.UpdateDate = executionContext.ExecutionDate;

        await _dbContext.SaveChangesAsync();

        await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(dbResult.PageId, command.PageVersionBlockId));
    }

    private Task OnTransactionComplete(int pageId, int pageVersionBlockId)
    {
        _pageCache.Clear(pageId);

        return _messageAggregator.PublishAsync(new PageVersionBlockMovedMessage()
        {
            PageId = pageId,
            PageVersionBlockId = pageVersionBlockId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(MovePageVersionBlockCommand command)
    {
        yield return new PageUpdatePermission();
    }
}