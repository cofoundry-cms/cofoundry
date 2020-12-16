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

namespace Cofoundry.Domain.Internal
{
    public class UpdatePageCommandHandler 
        : ICommandHandler<UpdatePageCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageCommand>
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execute

        public async Task ExecuteAsync(UpdatePageCommand command, IExecutionContext executionContext)
        {
            var page = await GetPageById(command.PageId).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(page, command.PageId);

            MapPage(command, executionContext, page);
            await _dbContext.SaveChangesAsync();

            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(page));
        }

        private Task OnTransactionComplete(Page page)
        {
            _pageCache.Clear(page.PageId);

            return _messageAggregator.PublishAsync(new PageUpdatedMessage()
            {
                PageId = page.PageId,
                HasPublishedVersionChanged = page.PublishStatusCode == PublishStatusCode.Published
            });
        }

        #endregion

        #region helpers

        private IQueryable<Page> GetPageById(int id)
        {
            return _dbContext
                .Pages
                .Include(p => p.PageTags)
                .ThenInclude(a => a.Tag)
                .FilterActive()
                .FilterByPageId(id)
                .Where(p => p.PageId == id);
        }

        private void MapPage(UpdatePageCommand command, IExecutionContext executionContext, Page page)
        {
            _entityTagHelper.UpdateTags(page.PageTags, command.Tags, executionContext);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
