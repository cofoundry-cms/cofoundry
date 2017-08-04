using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class MoveCustomEntityVersionPageBlockCommandHandler
        : IAsyncCommandHandler<MoveCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;

        public MoveCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(MoveCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .CustomEntityVersionPageBlocks
                .Where(b => b.CustomEntityVersionPageBlockId == command.CustomEntityVersionPageBlockId)
                .Select(b => new
                {
                    Block = b,
                    CustomEntityId = b.CustomEntityVersion.CustomEntityId,
                    CustomEntityDefinitionCode = b.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                    WorkFlowStatusId = b.CustomEntityVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbResult, command.CustomEntityVersionPageBlockId);
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode);

            if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be moved unless the entity is in draft status");
            }

            var block = dbResult.Block;
            var blockToSwapWithQuery =  _dbContext
                .CustomEntityVersionPageBlocks
                .Where(p => p.PageTemplateRegionId == block.PageTemplateRegionId && p.CustomEntityVersionId == block.CustomEntityVersionId);

            CustomEntityVersionPageBlock blockToSwapWith;

            switch (command.Direction)
            {
                case OrderedItemMoveDirection.Up:
                    blockToSwapWith = await blockToSwapWithQuery
                        .Where(p => p.Ordering < block.Ordering)
                        .OrderByDescending(p => p.Ordering)
                        .FirstOrDefaultAsync();
                    break;
                case OrderedItemMoveDirection.Down:
                    blockToSwapWith = await blockToSwapWithQuery
                        .Where(p =>  p.Ordering > block.Ordering)
                        .OrderBy(p => p.Ordering)
                        .FirstOrDefaultAsync();
                    break;
                default:
                    throw new InvalidOperationException("OrderedItemMoveDirection not recognised: " + command.Direction);
            }

            if (blockToSwapWith == null) return;

            int oldOrdering = block.Ordering;
            block.Ordering = blockToSwapWith.Ordering;
            blockToSwapWith.Ordering = oldOrdering;

            await _dbContext.SaveChangesAsync();
            _customEntityCache.Clear(dbResult.CustomEntityDefinitionCode, dbResult.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityVersionBlockMovedMessage()
            {
                CustomEntityId = dbResult.CustomEntityId,
                CustomEntityDefinitionCode = dbResult.CustomEntityDefinitionCode,
                CustomEntityVersionBlockId = dbResult.Block.CustomEntityVersionPageBlockId
            });
        }

        #endregion
    }
}
