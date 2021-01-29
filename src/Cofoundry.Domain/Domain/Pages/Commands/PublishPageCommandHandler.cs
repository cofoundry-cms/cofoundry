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
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Publishes a page. If the page is already published and
    /// a date is specified then the publish date will be updated.
    /// </summary>
    public class PublishPageCommandHandler 
        : ICommandHandler<PublishPageCommand>
        , IPermissionRestrictedCommandHandler<PublishPageCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public PublishPageCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPageStoredProcedures pageStoredProcedures
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
            _pageStoredProcedures = pageStoredProcedures;
        }

        #region execution

        public async Task ExecuteAsync(PublishPageCommand command, IExecutionContext executionContext)
        {
            var version = await _dbContext
                .PageVersions
                .Include(p => p.Page)
                .FilterActive()
                .FilterByPageId(command.PageId)
                .OrderByLatest()
                .FirstOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(version, command.PageId);

            UpdatePublishDate(command, executionContext, version);

            if (version.WorkFlowStatusId == (int)WorkFlowStatus.Published
                && version.Page.PublishStatusCode == PublishStatusCode.Published)
            {
                // only thing we can do with a published version is update the date
                await _dbContext.SaveChangesAsync();

                await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command));
            }
            else
            {
                version.WorkFlowStatusId = (int)WorkFlowStatus.Published;
                version.Page.PublishStatusCode = PublishStatusCode.Published;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(PublishPageCommand command)
        {
            _pageCache.Clear();

            return _messageAggregator.PublishAsync(new PagePublishedMessage()
            {
                PageId = command.PageId
            });
        }

        private static void UpdatePublishDate(PublishPageCommand command, IExecutionContext executionContext, PageVersion draftVersion)
        {
            if (command.PublishDate.HasValue)
            {
                draftVersion.Page.PublishDate = command.PublishDate;
            }
            else if (!draftVersion.Page.PublishDate.HasValue)
            {
                draftVersion.Page.PublishDate = executionContext.ExecutionDate;
            }
        }


        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(PublishPageCommand command)
        {
            yield return new PagePublishPermission();
        }

        #endregion
    }
}
