using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Sets the status of a page to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    public class UnPublishPageCommandHandler 
        : ICommandHandler<UnPublishPageCommand>
        , IPermissionRestrictedCommandHandler<UnPublishPageCommand>
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public UnPublishPageCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPageStoredProcedures pageStoredProcedures
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _pageStoredProcedures = pageStoredProcedures;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UnPublishPageCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .FilterActive()
                .FilterByPageId(command.PageId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(page, command.PageId);

            if (page.PublishStatusCode == PublishStatusCode.Unpublished)
            {
                // No action
                return;
            }

            var version = await _dbContext
                .PageVersions
                .Include(p => p.Page)
                .FilterActive()
                .FilterByPageId(command.PageId)
                .OrderByLatest()
                .FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(version, command.PageId);

            page.PublishStatusCode = PublishStatusCode.Unpublished;
            version.WorkFlowStatusId = (int)WorkFlowStatus.Draft;

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                scope.QueueCompletionTask(() => OnTransactionComplete(command));

                await scope.CompleteAsync();
            }
        }

        private Task OnTransactionComplete(UnPublishPageCommand command)
        {
            _pageCache.Clear();

            return _messageAggregator.PublishAsync(new PageUnPublishedMessage()
            {
                PageId = command.PageId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UnPublishPageCommand command)
        {
            yield return new PagePublishPermission();
        }

        #endregion
    }
}
