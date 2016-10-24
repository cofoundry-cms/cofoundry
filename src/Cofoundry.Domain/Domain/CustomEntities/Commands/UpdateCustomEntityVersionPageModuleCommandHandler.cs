using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdateCustomEntityVersionPageModuleCommandHandler 
        : IAsyncCommandHandler<UpdateCustomEntityVersionPageModuleCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IPageModuleCommandHelper _pageModuleCommandHelper;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UpdateCustomEntityVersionPageModuleCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            ICustomEntityCache pageCache,
            IPageModuleCommandHelper pageModuleCommandHelper,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _customEntityCache = pageCache;
            _pageModuleCommandHelper = pageModuleCommandHelper;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext)
        {
            var dbModule = await _dbContext
                .CustomEntityVersionPageModules
                .Include(m => m.PageModuleTypeTemplate)
                .Include(m => m.PageModuleType)
                .Include(m => m.CustomEntityVersion)
                .Include(m => m.CustomEntityVersion.CustomEntity)
                .Where(l => l.CustomEntityVersionPageModuleId == command.CustomEntityVersionPageModuleId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbModule, command.CustomEntityVersionPageModuleId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbModule.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode);

            using (var scope = _transactionScopeFactory.Create())
            {
                await _pageModuleCommandHelper.UpdateModelAsync(command, dbModule);
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionPageModuleEntityDefinition.DefinitionCode,
                    dbModule.CustomEntityVersionPageModuleId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);

                scope.Complete();
            }

            _customEntityCache.Clear(dbModule.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode, dbModule.CustomEntityVersion.CustomEntity.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityVersionModuleUpdatedMessage()
            {
                CustomEntityId = dbModule.CustomEntityVersion.CustomEntityId,
                CustomEntityDefinitionCode = dbModule.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityVersionModuleId = dbModule.CustomEntityVersionPageModuleId,
                HasPublishedVersionChanged = dbModule.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published
            });
        }

        #endregion
    }
}
