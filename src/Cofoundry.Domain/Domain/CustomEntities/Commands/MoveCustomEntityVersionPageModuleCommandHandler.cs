using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class MoveCustomEntityVersionPageModuleCommandHandler 
        : IAsyncCommandHandler<MoveCustomEntityVersionPageModuleCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;

        public MoveCustomEntityVersionPageModuleCommandHandler(
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

        public async Task ExecuteAsync(MoveCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext)
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
            EntityNotFoundException.ThrowIfNull(dbResult, command.CustomEntityVersionPageModuleId);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(dbResult.CustomEntityDefinitionCode);

            var module = dbResult.Module;
            var moduleToSwapWithQuery =  _dbContext
                .CustomEntityVersionPageModules
                .Where(p => p.PageTemplateSectionId == module.PageTemplateSectionId && p.CustomEntityVersionId == module.CustomEntityVersionId);

            CustomEntityVersionPageModule moduleToSwapWith;

            switch (command.Direction)
            {
                case OrderedItemMoveDirection.Up:
                    moduleToSwapWith = await moduleToSwapWithQuery
                        .Where(p => p.Ordering < module.Ordering)
                        .OrderByDescending(p => p.Ordering)
                        .FirstOrDefaultAsync();
                    break;
                case OrderedItemMoveDirection.Down:
                    moduleToSwapWith = await moduleToSwapWithQuery
                        .Where(p =>  p.Ordering > module.Ordering)
                        .OrderBy(p => p.Ordering)
                        .FirstOrDefaultAsync();
                    break;
                default:
                    throw new InvalidOperationException("OrderedItemMoveDirection not recognised: " + command.Direction);
            }

            if (moduleToSwapWith == null) return;

            int oldOrdering = module.Ordering;
            module.Ordering = moduleToSwapWith.Ordering;
            moduleToSwapWith.Ordering = oldOrdering;

            await _dbContext.SaveChangesAsync();
            _customEntityCache.Clear(dbResult.CustomEntityDefinitionCode, dbResult.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityVersionModuleMovedMessage()
            {
                CustomEntityId = dbResult.CustomEntityId,
                CustomEntityDefinitionCode = dbResult.CustomEntityDefinitionCode,
                CustomEntityVersionModuleId = dbResult.Module.CustomEntityVersionPageModuleId,
                HasPublishedVersionChanged = dbResult.WorkFlowStatusId == (int)WorkFlowStatus.Published
            });
        }

        #endregion
    }
}
