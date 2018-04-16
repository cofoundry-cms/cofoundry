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
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class UnPublishPageCommandHandler 
        : IAsyncCommandHandler<UnPublishPageCommand>
        , IPermissionRestrictedCommandHandler<UnPublishPageCommand>
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public UnPublishPageCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory,
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
                .Where(v => v.PageId == command.PageId
                    && !v.IsDeleted
                    && !v.Page.IsDeleted
                    && (v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published))
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate)
                .FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(version, command.PageId);

            page.PublishStatusCode = PublishStatusCode.Unpublished;
            version.WorkFlowStatusId = (int)WorkFlowStatus.Draft;

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);
                scope.Complete();
            }

            _pageCache.Clear();

            await _messageAggregator.PublishAsync(new PageUnPublishedMessage()
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
