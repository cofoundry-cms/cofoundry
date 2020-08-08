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
    /// Updates the draft version of a custom entity. If a draft version
    /// does not exist then one is created first from the currently
    /// published version.
    /// </summary>
    public class UpdatePageDraftVersionCommandHandler 
        : ICommandHandler<UpdatePageDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageDraftVersionCommand>
    {
        #region constructor
        
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public UpdatePageDraftVersionCommandHandler(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPageStoredProcedures pageStoredProcedures
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
            _pageStoredProcedures = pageStoredProcedures;
        }

        #endregion

        public async Task ExecuteAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext)
        {
            var draft = await GetDraftVersion(command.PageId).SingleOrDefaultAsync();

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                draft = await CreateDraftIfRequiredAsync(command.PageId, draft);
                UpdateDraft(command, draft);

                await _dbContext.SaveChangesAsync();
                await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                scope.QueueCompletionTask(() => OnTransactionComplete(draft));

                if (command.Publish)
                {
                    await _commandExecutor.ExecuteAsync(new PublishPageCommand(draft.PageId, command.PublishDate), executionContext);
                }

                await scope.CompleteAsync();
            }
        }

        private Task OnTransactionComplete(PageVersion draft)
        {
            _pageCache.Clear(draft.PageId);

            return _messageAggregator.PublishAsync(new PageDraftVersionUpdatedMessage()
            {
                PageId = draft.PageId,
                PageVersionId = draft.PageVersionId
            });
        }

        private IQueryable<PageVersion> GetDraftVersion(int pageId)
        {
            return _dbContext
                .PageVersions
                .Where(p => p.PageId == pageId && p.WorkFlowStatusId == (int)WorkFlowStatus.Draft);
        }

        private async Task<PageVersion> CreateDraftIfRequiredAsync(int pageId, PageVersion draft)
        {
            if (draft != null) return draft;

            var command = new AddPageDraftVersionCommand();
            command.PageId = pageId;
            await _commandExecutor.ExecuteAsync(command);

            return await GetDraftVersion(pageId).SingleOrDefaultAsync();
        }

        private void UpdateDraft(UpdatePageDraftVersionCommand command, PageVersion draft)
        {
            EntityNotFoundException.ThrowIfNull(draft, "Draft:" + command.PageId);

            draft.Title = command.Title;
            draft.ExcludeFromSitemap = !command.ShowInSiteMap;
            draft.MetaDescription = command.MetaDescription ?? string.Empty;
            draft.OpenGraphTitle = command.OpenGraphTitle;
            draft.OpenGraphDescription = command.OpenGraphDescription;
            draft.OpenGraphImageId = command.OpenGraphImageId;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDraftVersionCommand command)
        {
            yield return new PageUpdatePermission();

            if (command.Publish)
            {
                yield return new PagePublishPermission();
            }
        }

        #endregion
    }
}
