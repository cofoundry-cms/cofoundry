using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Deletes a block from a template region on a custom entity page.
    /// </summary>
    public class DeletePageVersionBlockCommandHandler
        : ICommandHandler<DeletePageVersionBlockCommand>
        , IPermissionRestrictedCommandHandler<DeletePageVersionBlockCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeletePageVersionBlockCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

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
                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageVersionBlockEntityDefinition.DefinitionCode, dbResult.Block.PageVersionBlockId), executionContext);

                    _dbContext.PageVersionBlocks.Remove(dbResult.Block);
                    await _dbContext.SaveChangesAsync();

                    scope.QueueCompletionTask(() => OnTransactionComplete(dbResult.PageId, versionId));

                    await scope.CompleteAsync();
                }
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

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageVersionBlockCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
