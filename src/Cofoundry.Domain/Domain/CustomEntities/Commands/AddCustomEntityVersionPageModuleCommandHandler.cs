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
    public class AddCustomEntityVersionPageModuleCommandHandler 
        : IAsyncCommandHandler<AddCustomEntityVersionPageModuleCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IPageModuleCommandHelper _pageModuleCommandHelper;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        
        public AddCustomEntityVersionPageModuleCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            EntityOrderableHelper entityOrderableHelper,
            ICustomEntityCache customEntityCache,
            IPageModuleCommandHelper pageModuleCommandHelper,
            IMessageAggregator messageAggregator,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _entityOrderableHelper = entityOrderableHelper;
            _customEntityCache = customEntityCache;
            _pageModuleCommandHelper = pageModuleCommandHelper;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext)
        {
            var customEntityVersion = _dbContext
                .CustomEntityVersions
                .Include(s => s.CustomEntityVersionPageModules)
                .Include(s => s.CustomEntity)
                .FirstOrDefault(v => v.CustomEntityVersionId == command.CustomEntityVersionId);

            EntityNotFoundException.ThrowIfNull(customEntityVersion, command.CustomEntityVersionId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode);

            var templateSectionSection = await _dbContext
                .PageTemplateSections
                .FirstOrDefaultAsync(l => l.PageTemplateSectionId == command.PageTemplateSectionId);
            EntityNotFoundException.ThrowIfNull(templateSectionSection, command.PageTemplateSectionId);

            var customEntityVersionModules = customEntityVersion
                .CustomEntityVersionPageModules
                .Where(m => m.PageTemplateSectionId == templateSectionSection.PageTemplateSectionId);

            CustomEntityVersionPageModule adjacentItem = null;
            if (command.AdjacentVersionModuleId.HasValue)
            {
                adjacentItem = customEntityVersionModules
                    .SingleOrDefault(m => m.CustomEntityVersionPageModuleId == command.AdjacentVersionModuleId);
                EntityNotFoundException.ThrowIfNull(adjacentItem, command.AdjacentVersionModuleId);
            }

            var newModule = new CustomEntityVersionPageModule();
            newModule.PageTemplateSection = templateSectionSection;

            await _pageModuleCommandHelper.UpdateModelAsync(command, newModule);

            newModule.CustomEntityVersion = customEntityVersion;

            _entityOrderableHelper.SetOrderingForInsert(customEntityVersionModules, newModule, command.InsertMode, adjacentItem);

            _dbContext.CustomEntityVersionPageModules.Add(newModule);

            using (var scope = _transactionScopeFactory.Create())
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionPageModuleEntityDefinition.DefinitionCode,
                    newModule.CustomEntityVersionPageModuleId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);

                scope.Complete();
            }
            _customEntityCache.Clear(customEntityVersion.CustomEntity.CustomEntityDefinitionCode, customEntityVersion.CustomEntityId);

            command.OutputCustomEntityVersionId = newModule.CustomEntityVersionPageModuleId;

            await _messageAggregator.PublishAsync(new CustomEntityVersionModuleAddedMessage()
            {
                CustomEntityId = customEntityVersion.CustomEntityId,
                CustomEntityVersionPageModuleId = newModule.CustomEntityVersionPageModuleId,
                CustomEntityDefinitionCode = customEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                HasPublishedVersionChanged = customEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published
            });
        }

        #endregion
    }
}
