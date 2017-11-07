using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class PublishPageCommandHandler 
        : IAsyncCommandHandler<PublishPageCommand>
        , IPermissionRestrictedCommandHandler<PublishPageCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public PublishPageCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory,
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
            var version =await _dbContext
                .PageVersions
                .Include(p => p.Page)
                .Where(v => v.PageId == command.PageId
                    && !v.IsDeleted
                    && !v.Page.IsDeleted
                    && (v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published))
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate)
                .FirstOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(version, command.PageId);

            UpdatePublishDate(command, executionContext, version);

            if (version.WorkFlowStatusId == (int)WorkFlowStatus.Published)
            {
                // only thing we can do with a published version is update the date
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                version.WorkFlowStatusId = (int)WorkFlowStatus.Published;
                version.Page.PublishStatusCode = PublishStatusCode.Published;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);
                    scope.Complete();
                }
            }

            _pageCache.Clear();

            await _messageAggregator.PublishAsync(new PagePublishedMessage()
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
