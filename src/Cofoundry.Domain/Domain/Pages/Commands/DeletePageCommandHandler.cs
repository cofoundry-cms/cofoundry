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

namespace Cofoundry.Domain
{
    public class DeletePageCommandHandler 
        : IAsyncCommandHandler<DeletePageCommand>
        , IPermissionRestrictedCommandHandler<DeletePageCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public DeletePageCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory,
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

        public async Task ExecuteAsync(DeletePageCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .SingleOrDefaultAsync(p => p.PageId == command.PageId);

            if (page != null)
            {
                page.IsDeleted = true;
                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageEntityDefinition.DefinitionCode, command.PageId), executionContext);
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                    scope.Complete();
                }
                _pageCache.Clear(command.PageId);

                await _messageAggregator.PublishAsync(new PageDeletedMessage()
                {
                    PageId = command.PageId
                });
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageCommand command)
        {
            yield return new PageDeletePermission();
        }

        #endregion
    }
}
