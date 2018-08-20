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

namespace Cofoundry.Domain
{
    public class DeletePageDraftVersionCommandHandler
        : IAsyncCommandHandler<DeletePageDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<DeletePageDraftVersionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public DeletePageDraftVersionCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPageStoredProcedures pageStoredProcedures
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
            _pageStoredProcedures = pageStoredProcedures;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DeletePageDraftVersionCommand command, IExecutionContext executionContext)
        {
            var draft = await _dbContext
                .PageVersions
                .SingleOrDefaultAsync(v => v.PageId == command.PageId 
                                      && v.WorkFlowStatusId == (int)WorkFlowStatus.Draft 
                                      && !v.IsDeleted);

            if (draft != null)
            {
                var versionId = draft.PageVersionId;
                draft.IsDeleted = true;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageVersionEntityDefinition.DefinitionCode, draft.PageVersionId), executionContext);
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command, versionId));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(DeletePageDraftVersionCommand command, int versionId)
        {
            _pageCache.Clear(command.PageId);

            return _messageAggregator.PublishAsync(new PageDraftVersionDeletedMessage()
            {
                PageId = command.PageId,
                PageVersionId = versionId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDraftVersionCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
