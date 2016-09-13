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
    public class AddPageVersionModuleCommandHandler
        : IAsyncCommandHandler<AddPageVersionModuleCommand>
        , IPermissionRestrictedCommandHandler<AddPageVersionModuleCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityOrderableHelper _entityOrderableHelper;
        private readonly IPageCache _pageCache;
        private readonly PageModuleCommandHelper _pageModuleCommandHelper;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public AddPageVersionModuleCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityOrderableHelper entityOrderableHelper,
            IPageCache pageCache,
            PageModuleCommandHelper pageModuleCommandHelper,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityOrderableHelper = entityOrderableHelper;
            _pageCache = pageCache;
            _pageModuleCommandHelper = pageModuleCommandHelper;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(AddPageVersionModuleCommand command, IExecutionContext executionContext)
        {
            var templateSectionSection = await _dbContext
                .PageTemplateSections
                .FirstOrDefaultAsync(l => l.PageTemplateSectionId == command.PageTemplateSectionId);
            EntityNotFoundException.ThrowIfNull(templateSectionSection, command.PageTemplateSectionId);

            var pageVersion = _dbContext
                .PageVersions
                .Include(s => s.PageVersionModules)
                .FirstOrDefault(v => v.PageVersionId == command.PageVersionId);
            EntityNotFoundException.ThrowIfNull(pageVersion, command.PageVersionId);

            var pageVersionModules = pageVersion
                .PageVersionModules
                .Where(m => m.PageTemplateSectionId == templateSectionSection.PageTemplateSectionId);

            PageVersionModule adjacentItem = null;
            if (command.AdjacentVersionModuleId.HasValue)
            {
                adjacentItem = pageVersionModules
                    .SingleOrDefault(m => m.PageVersionModuleId == command.AdjacentVersionModuleId);
                EntityNotFoundException.ThrowIfNull(adjacentItem, command.AdjacentVersionModuleId);
            }

            var newModule = new PageVersionModule();
            newModule.PageTemplateSection = templateSectionSection;

            await _pageModuleCommandHelper.UpdateModelAsync(command, newModule);

            newModule.PageVersion = pageVersion;
            newModule.UpdateDate = executionContext.ExecutionDate;

            _entityAuditHelper.SetCreated(newModule, executionContext);
            _entityOrderableHelper.SetOrderingForInsert(pageVersionModules, newModule, command.InsertMode, adjacentItem);

            _dbContext.PageVersionModules.Add(newModule);
            using (var scope = _transactionScopeFactory.Create())
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    PageVersionModuleEntityDefinition.DefinitionCode,
                    newModule.PageVersionModuleId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);
                scope.Complete();
            }
            _pageCache.Clear(pageVersion.PageId);

            command.OutputPageModuleId = newModule.PageVersionModuleId;

            await _messageAggregator.PublishAsync(new PageVersionModuleAddedMessage()
            {
                PageId = pageVersion.PageId,
                PageVersionModuleId = newModule.PageVersionModuleId,
                HasPublishedVersionChanged = pageVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published
            });
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageVersionModuleCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
