using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a new block to a template region on a custom entity page.
    /// </summary>
    public class AddCustomEntityVersionPageBlockCommandHandler
        : ICommandHandler<AddCustomEntityVersionPageBlockCommand>
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
        private readonly ITransactionScopeManager _transactionScopeFactory;
        
        public AddCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            EntityOrderableHelper entityOrderableHelper,
            ICustomEntityCache customEntityCache,
            IPageBlockCommandHelper pageBlockCommandHelper,
            IMessageAggregator messageAggregator,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeFactory
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
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

            if (customEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be added unless the entity is in draft status");
            }

            var page = await _dbContext
                .Pages
                .FilterActive()
                .FilterByPageId(command.PageId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(page, command.PageId);

            var templateRegion = await _dbContext
                .PageTemplateRegions
                .FirstOrDefaultAsync(l => l.PageTemplateRegionId == command.PageTemplateRegionId);
            EntityNotFoundException.ThrowIfNull(templateRegion, command.PageTemplateRegionId);

            await ValidateTemplateUsedByPage(command, templateRegion);

            var customEntityVersionBlocks = customEntityVersion
                .CustomEntityVersionPageBlocks
                .Where(m => m.PageTemplateRegionId == templateRegion.PageTemplateRegionId);

            CustomEntityVersionPageBlock adjacentItem = null;
            if (command.AdjacentVersionBlockId.HasValue)
            {
                adjacentItem = customEntityVersionBlocks
                    .SingleOrDefault(m => m.CustomEntityVersionPageBlockId == command.AdjacentVersionBlockId);
                EntityNotFoundException.ThrowIfNull(adjacentItem, command.AdjacentVersionBlockId);

                if (adjacentItem.PageTemplateRegionId != command.PageTemplateRegionId)
                {
                    throw new Exception("Error adding custom entity page block: the block specified in AdjacentVersionBlockId is in a different region to the block being added.");
                }
            }

            var newBlock = new CustomEntityVersionPageBlock();
            newBlock.PageTemplateRegion = templateRegion;
            newBlock.Page = page;
            newBlock.CustomEntityVersion = customEntityVersion;

            await _pageBlockCommandHelper.UpdateModelAsync(command, newBlock);

            _entityOrderableHelper.SetOrderingForInsert(customEntityVersionBlocks, newBlock, command.InsertMode, adjacentItem);

            _dbContext.CustomEntityVersionPageBlocks.Add(newBlock);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionPageBlockEntityDefinition.DefinitionCode,
                    newBlock.CustomEntityVersionPageBlockId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(customEntityVersion, newBlock));

                await scope.CompleteAsync();
            }

            command.OutputCustomEntityVersionPageBlockId = newBlock.CustomEntityVersionPageBlockId;
        }

        private Task OnTransactionComplete(CustomEntityVersion customEntityVersion, CustomEntityVersionPageBlock newBlock)
        {
            _customEntityCache.Clear(customEntityVersion.CustomEntity.CustomEntityDefinitionCode, customEntityVersion.CustomEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityVersionBlockAddedMessage()
            {
                CustomEntityId = customEntityVersion.CustomEntityId,
                CustomEntityVersionPageBlockId = newBlock.CustomEntityVersionPageBlockId,
                CustomEntityDefinitionCode = customEntityVersion.CustomEntity.CustomEntityDefinitionCode
            });
        }

        private async Task ValidateTemplateUsedByPage(AddCustomEntityVersionPageBlockCommand command, PageTemplateRegion templateRegion)
        {
            var isRegionInTemplate = await _dbContext
                .PageVersions
                .FilterActive()
                .FilterByPageId(command.PageId)
                .AnyAsync(p => p.PageTemplateId == templateRegion.PageTemplateId);

            if (!isRegionInTemplate)
            {
                throw new Exception($"Error adding custom entity page block. The page template region {command.PageTemplateRegionId} does not belong to a template referenced by the page {command.PageId}");
            }
        }

        #endregion
    }
}
