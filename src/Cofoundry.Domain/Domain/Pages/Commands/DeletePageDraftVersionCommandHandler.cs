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
    public class DeletePageDraftVersionCommandHandler
        : IAsyncCommandHandler<DeletePageDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<DeletePageDraftVersionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DeletePageDraftVersionCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
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
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageVersionEntityDefinition.DefinitionCode, draft.PageVersionId));

                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _pageCache.Clear(command.PageId);

                await _messageAggregator.PublishAsync(new PageDraftVersionDeletedMessage()
                {
                    PageId = command.PageId,
                    PageVersionId = versionId
                });
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDraftVersionCommand command)
        {
            yield return new PageDeletePermission();
        }

        #endregion
    }
}
