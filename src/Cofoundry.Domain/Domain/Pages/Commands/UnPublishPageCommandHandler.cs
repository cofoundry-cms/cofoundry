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

namespace Cofoundry.Domain
{
    public class UnPublishPageCommandHandler 
        : IAsyncCommandHandler<UnPublishPageCommand>
        , IPermissionRestrictedCommandHandler<UnPublishPageCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;

        public UnPublishPageCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UnPublishPageCommand command, IExecutionContext executionContext)
        {
            var pageVersions = await _dbContext
                .PageVersions
                .Where(p => p.PageId == command.PageId 
                    && !p.IsDeleted 
                    && !p.Page.IsDeleted 
                    && (p.WorkFlowStatusId == (int)WorkFlowStatus.Published || p.WorkFlowStatusId == (int)WorkFlowStatus.Draft))
                .ToListAsync();

            var publishedVersion = pageVersions.SingleOrDefault(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            EntityNotFoundException.ThrowIfNull(publishedVersion, command.PageId);

            if (pageVersions.Any(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Draft))
            {
                // If there's already a draft, change to approved.
                publishedVersion.WorkFlowStatusId = (int)WorkFlowStatus.Approved;
            }
            else
            {
                // Else set it to draft
                publishedVersion.WorkFlowStatusId = (int)WorkFlowStatus.Draft;
            }

            await _dbContext.SaveChangesAsync();
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
