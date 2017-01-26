using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DeletePageVersionModuleCommandHandler
        : IAsyncCommandHandler<DeletePageVersionModuleCommand>
        , IPermissionRestrictedCommandHandler<DeletePageVersionModuleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DeletePageVersionModuleCommandHandler(
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

        public async Task ExecuteAsync(DeletePageVersionModuleCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageVersionModules
                .Where(p => p.PageVersionModuleId == command.PageVersionModuleId)
                .Select(p => new 
                {
                    Module = p,
                    PageId = p.PageVersion.PageId,
                    WorkFlowStatusId = p.PageVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();

            if (dbResult != null)
            {
                if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
                {
                    throw new NotPermittedException("Page modules cannot be deleted unless the page version is in draft status");
                }

                var versionId = dbResult.Module.PageVersionId;
                using (var scope = _transactionScopeFactory.Create())
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(PageVersionModuleEntityDefinition.DefinitionCode, dbResult.Module.PageVersionModuleId));

                    _dbContext.PageVersionModules.Remove(dbResult.Module);
                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _pageCache.Clear(dbResult.PageId);

                await _messageAggregator.PublishAsync(new PageVersionModuleDeletedMessage()
                {
                    PageId = dbResult.PageId,
                    PageVersionId = versionId
                });
                
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageVersionModuleCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
