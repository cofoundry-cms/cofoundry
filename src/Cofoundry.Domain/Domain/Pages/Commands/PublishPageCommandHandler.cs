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
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public PublishPageCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #region execution

        public async Task ExecuteAsync(PublishPageCommand command, IExecutionContext executionContext)
        {
            var pageVersions = await QueryVersions(command).ToListAsync();

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                // Find the published one and make it appPageroved
                var publishedVersion = pageVersions.SingleOrDefault(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                if (publishedVersion != null)
                {
                    publishedVersion.WorkFlowStatusId = (int)WorkFlowStatus.Approved;
                    await _dbContext.SaveChangesAsync();
                }

                // Find the draft page and make it published
                SetDraftVersionPublished(command, pageVersions);
                await _dbContext.SaveChangesAsync();

                scope.Complete();
            }

            _pageCache.Clear();

            await _messageAggregator.PublishAsync(new PagePublishedMessage()
            {
                PageId = command.PageId
            });
        }

        #endregion

        #region helpers

        private IQueryable<PageVersion> QueryVersions(PublishPageCommand command)
        {
            return _dbContext
                .PageVersions
                .Where(p => p.PageId == command.PageId
                    && !p.IsDeleted
                    && !p.Page.IsDeleted
                    && (p.WorkFlowStatusId == (int)WorkFlowStatus.Draft || p.WorkFlowStatusId == (int)WorkFlowStatus.Published));
        }

        private void SetDraftVersionPublished(PublishPageCommand command, List<PageVersion> pageVersions)
        {
            var draftVersion = pageVersions.SingleOrDefault(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);
            EntityNotFoundException.ThrowIfNull(draftVersion, "Draft:" + command.PageId);
            draftVersion.WorkFlowStatusId = (int)WorkFlowStatus.Published;
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
