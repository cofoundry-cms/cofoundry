using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
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
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public AddPageVersionBlockCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            IPageCache pageCache,
            IPageBlockCommandHelper pageBlockCommandHelper,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _pageCache = pageCache;
            _pageBlockCommandHelper = pageBlockCommandHelper;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
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

            var pageVersionBlocks = pageVersion
                .PageVersionBlocks
                .Where(m => m.PageTemplateRegionId == templateRegion.PageTemplateRegionId);

            PageVersionBlock adjacentItem = null;
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
            using (var scope = _transactionScopeFactory.Create(_dbContext))
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

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageVersionBlockCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
