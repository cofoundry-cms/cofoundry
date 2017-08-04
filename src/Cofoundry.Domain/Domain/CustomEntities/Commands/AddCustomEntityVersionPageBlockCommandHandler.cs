using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddCustomEntityVersionPageBlockCommandHandler
        : IAsyncCommandHandler<AddCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IPageBlockCommandHelper _pageBlockCommandHelper;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        
        public AddCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            EntityOrderableHelper entityOrderableHelper,
            ICustomEntityCache customEntityCache,
            IPageBlockCommandHelper pageBlockCommandHelper,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _entityOrderableHelper = entityOrderableHelper;
            _customEntityCache = customEntityCache;
            _pageBlockCommandHelper = pageBlockCommandHelper;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
        {
            var customEntityVersion = _dbContext
                .CustomEntityVersions
                .Include(s => s.CustomEntityVersionPageBlocks)
                .Include(s => s.CustomEntity)
                .FirstOrDefault(v => v.CustomEntityVersionId == command.CustomEntityVersionId);

            EntityNotFoundException.ThrowIfNull(customEntityVersion, command.CustomEntityVersionId);
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityUpdatePermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode);

            if (customEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be deleted unless the entity is in draft status");
            }

            var templateRegion = await _dbContext
                .PageTemplateRegions
                .FirstOrDefaultAsync(l => l.PageTemplateRegionId == command.PageTemplateRegionId);
            EntityNotFoundException.ThrowIfNull(templateRegion, command.PageTemplateRegionId);

            var customEntityVersionBlocks = customEntityVersion
                .CustomEntityVersionPageBlocks
                .Where(m => m.PageTemplateRegionId == templateRegion.PageTemplateRegionId);

            CustomEntityVersionPageBlock adjacentItem = null;
            if (command.AdjacentVersionBlockId.HasValue)
            {
                adjacentItem = customEntityVersionBlocks
                    .SingleOrDefault(m => m.CustomEntityVersionPageBlockId == command.AdjacentVersionBlockId);
                EntityNotFoundException.ThrowIfNull(adjacentItem, command.AdjacentVersionBlockId);
            }

            var newBlock = new CustomEntityVersionPageBlock();
            newBlock.PageTemplateRegion = templateRegion;

            await _pageBlockCommandHelper.UpdateModelAsync(command, newBlock);

            newBlock.CustomEntityVersion = customEntityVersion;

            _entityOrderableHelper.SetOrderingForInsert(customEntityVersionBlocks, newBlock, command.InsertMode, adjacentItem);

            _dbContext.CustomEntityVersionPageBlocks.Add(newBlock);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionPageBlockEntityDefinition.DefinitionCode,
                    newBlock.CustomEntityVersionPageBlockId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);

                scope.Complete();
            }
            _customEntityCache.Clear(customEntityVersion.CustomEntity.CustomEntityDefinitionCode, customEntityVersion.CustomEntityId);

            command.OutputCustomEntityVersionPageBlockId = newBlock.CustomEntityVersionPageBlockId;

            await _messageAggregator.PublishAsync(new CustomEntityVersionBlockAddedMessage()
            {
                CustomEntityId = customEntityVersion.CustomEntityId,
                CustomEntityVersionPageBlockId = newBlock.CustomEntityVersionPageBlockId,
                CustomEntityDefinitionCode = customEntityVersion.CustomEntity.CustomEntityDefinitionCode
            });
        }

        #endregion
    }
}
