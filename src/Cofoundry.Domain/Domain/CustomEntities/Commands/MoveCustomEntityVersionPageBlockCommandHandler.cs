using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Moves a block up or down within a multi-block region 
    /// on a custom entity page.
    /// </summary>
    public class MoveCustomEntityVersionPageBlockCommandHandler
        : ICommandHandler<MoveCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public MoveCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

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
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode, executionContext.UserContext);

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

            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(
                dbResult.CustomEntityDefinitionCode,
                dbResult.CustomEntityId,
                dbResult.Block.CustomEntityVersionPageBlockId)
                );
        }

        private Task OnTransactionComplete(string customEntityDefinitionCode, int customEntityId, int customEntityVersionPageBlockId)
        {
            _customEntityCache.Clear(customEntityDefinitionCode, customEntityId);

            return _messageAggregator.PublishAsync(new CustomEntityVersionBlockMovedMessage()
            {
                CustomEntityId = customEntityId,
                CustomEntityDefinitionCode = customEntityDefinitionCode,
                CustomEntityVersionBlockId = customEntityVersionPageBlockId
            });
        }
    }
}
