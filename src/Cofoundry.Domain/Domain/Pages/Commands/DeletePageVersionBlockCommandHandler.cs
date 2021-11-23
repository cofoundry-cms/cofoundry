using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Deletes a block from a template region on a custom entity page.
    /// </summary>
    public class DeletePageVersionBlockCommandHandler
        : ICommandHandler<DeletePageVersionBlockCommand>
        , IPermissionRestrictedCommandHandler<DeletePageVersionBlockCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeletePageVersionBlockCommandHandler(
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

        public async Task ExecuteAsync(DeletePageVersionBlockCommand command, IExecutionContext executionContext)
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

            if (dbResult != null)
            {
                if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
                {
                    throw new NotPermittedException("Page blocks cannot be deleted unless the page version is in draft status");
                }

                var versionId = dbResult.Block.PageVersionId;
                _dbContext.PageVersionBlocks.Remove(dbResult.Block);

                await _dbContext.SaveChangesAsync();

                _transactionScopeFactory.QueueCompletionTask(_dbContext, () => OnTransactionComplete(dbResult.PageId, versionId));
            }
        }

        private Task OnTransactionComplete(
            int pageId,
            int pageVersionId
            )
        {
            _pageCache.Clear(pageId);

            return _messageAggregator.PublishAsync(new PageVersionBlockDeletedMessage()
            {
                PageId = pageId,
                PageVersionId = pageVersionId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageVersionBlockCommand command)
        {
            yield return new PageUpdatePermission();
        }
    }
}
