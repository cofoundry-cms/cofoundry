using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class DeleteCustomEntityDraftVersionCommandHandler 
        : IAsyncCommandHandler<DeleteCustomEntityDraftVersionCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DeleteCustomEntityDraftVersionCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
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

        #region execution

        public async Task ExecuteAsync(DeleteCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
        {
            var draft = await _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntity)
                .SingleOrDefaultAsync(v => v.CustomEntityId == command.CustomEntityId 
                                      && v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);

            if (draft != null)
            {
                _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(draft.CustomEntity.CustomEntityDefinitionCode);

                var definitionCode = draft.CustomEntity.CustomEntityDefinitionCode;
                var versionId = draft.CustomEntityVersionId;

                using (var scope = _transactionScopeFactory.Create())
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(CustomEntityVersionEntityDefinition.DefinitionCode, draft.CustomEntityVersionId));

                    _dbContext.CustomEntityVersions.Remove(draft);
                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _customEntityCache.Clear(definitionCode, command.CustomEntityId);

                await _messageAggregator.PublishAsync(new CustomEntityDraftVersionDeletedMessage()
                {
                    CustomEntityId = command.CustomEntityId,
                    CustomEntityDefinitionCode = definitionCode,
                    CustomEntityVersionId = versionId
                });
            }
        }

        #endregion
    }
}
