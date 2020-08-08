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
    /// Deletes the draft verison of a custom entity permanently if 
    /// it exists. If no draft exists then no action is taken.
    /// </summary>
    public class DeleteCustomEntityDraftVersionCommandHandler 
        : ICommandHandler<DeleteCustomEntityDraftVersionCommand>
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

        public DeleteCustomEntityDraftVersionCommandHandler(
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

        public async Task ExecuteAsync(DeleteCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
        {
            var draft = await _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntity)
                .SingleOrDefaultAsync(v => v.CustomEntityId == command.CustomEntityId 
                                      && v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);

            if (draft != null)
            {
                _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(draft.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

                var definitionCode = draft.CustomEntity.CustomEntityDefinitionCode;
                var versionId = draft.CustomEntityVersionId;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(CustomEntityVersionEntityDefinition.DefinitionCode, draft.CustomEntityVersionId), executionContext);

                    _dbContext.CustomEntityVersions.Remove(draft);
                    await _dbContext.SaveChangesAsync();
                    await _customEntityStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.CustomEntityId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command, definitionCode, versionId));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(
            DeleteCustomEntityDraftVersionCommand command, 
            string definitionCode, 
            int versionId
            )
        {
            _customEntityCache.Clear(definitionCode, command.CustomEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityDraftVersionDeletedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = definitionCode,
                CustomEntityVersionId = versionId
            });
        }
    }
}
