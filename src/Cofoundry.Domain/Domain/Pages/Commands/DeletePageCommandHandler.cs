using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class DeletePageCommandHandler
        : ICommandHandler<DeletePageCommand>
        , IPermissionRestrictedCommandHandler<DeletePageCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public DeletePageCommandHandler(
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

        public async Task ExecuteAsync(DeletePageCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .FilterByPageId(command.PageId)
                .SingleOrDefaultAsync();

            if (page != null)
            {
                _dbContext.Pages.Remove(page);

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageEntityDefinition.DefinitionCode, command.PageId), executionContext);
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(DeletePageCommand command)
        {
            _pageCache.Clear(command.PageId);

            return _messageAggregator.PublishAsync(new PageDeletedMessage()
            {
                PageId = command.PageId
            });
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageCommand command)
        {
            yield return new PageDeletePermission();
        }

        #endregion
    }
}
