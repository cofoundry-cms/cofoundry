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
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DeleteCustomEntityVersionPageModuleCommandHandler 
        : IAsyncCommandHandler<DeleteCustomEntityVersionPageModuleCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DeleteCustomEntityVersionPageModuleCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
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

        public async Task ExecuteAsync(DeleteCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .CustomEntityVersionPageModules
                .Where(m => m.CustomEntityVersionPageModuleId == command.CustomEntityVersionPageModuleId)
                .Select(m => new
                {
                    Module = m,
                    CustomEntityId = m.CustomEntityVersion.CustomEntityId,
                    CustomEntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                    WorkFlowStatusId = m.CustomEntityVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();

            if (dbResult != null)
            {
                await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode);

                if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
                {
                    throw new NotPermittedException("Page modules cannot be deleted unless the entity is in draft status");
                }

                var customEntityVersionModuleId = dbResult.Module.CustomEntityVersionPageModuleId;

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(CustomEntityVersionPageModuleEntityDefinition.DefinitionCode, dbResult.Module.CustomEntityVersionPageModuleId));

                    _dbContext.CustomEntityVersionPageModules.Remove(dbResult.Module);
                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _customEntityCache.Clear(dbResult.CustomEntityDefinitionCode, dbResult.CustomEntityId);

                await _messageAggregator.PublishAsync(new CustomEntityVersionModuleDeletedMessage()
                {
                    CustomEntityId = dbResult.CustomEntityId,
                    CustomEntityDefinitionCode = dbResult.CustomEntityDefinitionCode,
                    CustomEntityVersionId = customEntityVersionModuleId
                });
            }
        }

        #endregion
    }
}
