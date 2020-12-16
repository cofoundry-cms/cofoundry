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

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Deletes a block from a template region on a custom entity page.
    /// </summary>
    public class DeleteCustomEntityVersionPageBlockCommandHandler
        : ICommandHandler<DeleteCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeleteCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        public async Task ExecuteAsync(DeleteCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .CustomEntityVersionPageBlocks
                .Where(m => m.CustomEntityVersionPageBlockId == command.CustomEntityVersionPageBlockId)
                .Select(m => new
                {
                    Block = m,
                    CustomEntityId = m.CustomEntityVersion.CustomEntityId,
                    CustomEntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                    WorkFlowStatusId = m.CustomEntityVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();

            if (dbResult != null)
            {
                _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode, executionContext.UserContext);

                if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
                {
                    throw new NotPermittedException("Page blocks cannot be deleted unless the entity is in draft status");
                }

                var customEntityVersionBlockId = dbResult.Block.CustomEntityVersionPageBlockId;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(CustomEntityVersionPageBlockEntityDefinition.DefinitionCode, dbResult.Block.CustomEntityVersionPageBlockId), executionContext);

                    _dbContext.CustomEntityVersionPageBlocks.Remove(dbResult.Block);
                    await _dbContext.SaveChangesAsync();

                    scope.QueueCompletionTask(() => OnTransactionComplete(dbResult.CustomEntityDefinitionCode, dbResult.CustomEntityId, customEntityVersionBlockId));

                    await scope.CompleteAsync();
                }
                
            }
        }

        private Task OnTransactionComplete(
            string customEntityDefinitionCode, 
            int customEntityId, 
            int customEntityVersionBlockId
            )
        {
            _customEntityCache.Clear(customEntityDefinitionCode, customEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityVersionBlockDeletedMessage()
            {
                CustomEntityId = customEntityId,
                CustomEntityDefinitionCode = customEntityDefinitionCode,
                CustomEntityVersionId = customEntityVersionBlockId
            });
        }
    }
}
