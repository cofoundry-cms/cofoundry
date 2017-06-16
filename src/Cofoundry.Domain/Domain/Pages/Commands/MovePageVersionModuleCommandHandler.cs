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
    public class MovePageVersionModuleCommandHandler 
        : IAsyncCommandHandler<MovePageVersionModuleCommand>
        , IPermissionRestrictedCommandHandler<MovePageVersionModuleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;

        public MovePageVersionModuleCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(MovePageVersionModuleCommand command, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageVersionModules
                .Where(m => m.PageVersionModuleId == command.PageVersionModuleId)
                .Select(m => new
                {
                    Module = m,
                    PageId = m.PageVersion.PageId,
                    WorkFlowStatusId = m.PageVersion.WorkFlowStatusId
                })
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbResult, command.PageVersionModuleId);

            if (dbResult.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page modules cannot be moved unless the page version is in draft status");
            }

            var module = dbResult.Module;
            var moduleToSwapWithQuery =  _dbContext
                .PageVersionModules
                .Where(p => p.PageTemplateSectionId == module.PageTemplateSectionId && p.PageVersionId == module.PageVersionId);
            
            PageVersionModule moduleToSwapWith;

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
            module.UpdateDate = executionContext.ExecutionDate;
            moduleToSwapWith.Ordering = oldOrdering;
            moduleToSwapWith.UpdateDate = executionContext.ExecutionDate;

            await _dbContext.SaveChangesAsync();
            _pageCache.Clear(dbResult.PageId);

            await _messageAggregator.PublishAsync(new PageVersionModuleMovedMessage()
            {
                PageId = dbResult.PageId,
                PageVersionModuleId = command.PageVersionModuleId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(MovePageVersionModuleCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
