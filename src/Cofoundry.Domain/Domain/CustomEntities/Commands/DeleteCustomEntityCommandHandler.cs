using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Deletes a custom entity and all associated versions permanently.
    /// </summary>
    public class DeleteCustomEntityCommandHandler 
        : ICommandHandler<DeleteCustomEntityCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;

        public DeleteCustomEntityCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeFactory,
            ICustomEntityStoredProcedures customEntityStoredProcedures
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
            _customEntityStoredProcedures = customEntityStoredProcedures;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DeleteCustomEntityCommand command, IExecutionContext executionContext)
        {
            var customEntity = await _dbContext
                .CustomEntities
                .SingleOrDefaultAsync(p => p.CustomEntityId == command.CustomEntityId);

            if (customEntity != null)
            {
                _permissionValidationService.EnforceCustomEntityPermission<CustomEntityDeletePermission>(customEntity.CustomEntityDefinitionCode, executionContext.UserContext);

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(customEntity.CustomEntityDefinitionCode, customEntity.CustomEntityId), executionContext);

                    _dbContext.CustomEntities.Remove(customEntity);

                    await _dbContext.SaveChangesAsync();

                    await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.CustomEntityId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command, customEntity));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(DeleteCustomEntityCommand command, CustomEntity customEntity)
        {
            _customEntityCache.Clear(customEntity.CustomEntityDefinitionCode, command.CustomEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityDeletedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = customEntity.CustomEntityDefinitionCode
            });
        }

        #endregion
    }
}
