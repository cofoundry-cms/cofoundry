using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    public class MovePageVersionBlockCommandHandler
        : ICommandHandler<MovePageVersionBlockCommand>
        , IPermissionRestrictedCommandHandler<MovePageVersionBlockCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public MovePageVersionBlockCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(MovePageVersionBlockCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageVersionBlocks
                .Where(b => b.PageVersionBlockId == command.PageVersionBlockId)
                .Select(b => new
                {
                    Block = b,
                    PageId = b.PageVersion.PageId,
                    WorkFlowStatusId = b.PageVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbResult, command.PageVersionBlockId);

            if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be moved unless the page version is in draft status");
            }

            var block = dbResult.Block;
            var blockToSwapWithQuery =  _dbContext
                .PageVersionBlocks
                .Where(p => p.PageTemplateRegionId == block.PageTemplateRegionId && p.PageVersionId == block.PageVersionId);
            
            PageVersionBlock blockToSwapWith;

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
                        .Where(p =>  p.Ordering > block.Ordering)
                        .OrderBy(p => p.Ordering)
                        .FirstOrDefaultAsync();
                    break;
                default:
                    throw new InvalidOperationException("OrderedItemMoveDirection not recognized: " + command.Direction);
            }

            if (blockToSwapWith == null) return;

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

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(MovePageVersionBlockCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
