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
    public class UpdatePageVersionModuleCommandHandler 
        : IAsyncCommandHandler<UpdatePageVersionModuleCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageVersionModuleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UpdatePageVersionModuleCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _commandExecutor = commandExecutor;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdatePageVersionModuleCommand command, IExecutionContext executionContext)
        {
            var dbModule = await _dbContext
                .PageVersionModules
                .Include(m => m.PageModuleTypeTemplate)
                .Include(m => m.PageModuleType)
                .Include(m => m.PageVersion)
                .Where(l => l.PageVersionModuleId == command.PageVersionModuleId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbModule, command.PageVersionModuleId);

            if (command.PageModuleTypeId != dbModule.PageModuleTypeId)
            {
                var pageModuleType = await _dbContext
                    .PageModuleTypes
                    .Where(m => m.PageModuleTypeId == command.PageModuleTypeId && m.PageTemplateSections.Any(s => s.PageTemplateSectionId == dbModule.PageTemplateSectionId))
                    .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(pageModuleType, command.PageModuleTypeId);
                dbModule.PageModuleType = pageModuleType;
            }

            dbModule.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.DataModel);
            dbModule.UpdateDate = executionContext.ExecutionDate;

            if (command.PageModuleTypeTemplateId != dbModule.PageModuleTypeTemplateId && command.PageModuleTypeTemplateId.HasValue)
            {
                dbModule.PageModuleTypeTemplate = await _dbContext
                    .PageModuleTypeTemplates
                    .SingleOrDefaultAsync(m => m.PageModuleTypeId == dbModule.PageModuleTypeId && m.PageModuleTypeTemplateId == command.PageModuleTypeTemplateId);
                EntityNotFoundException.ThrowIfNull(dbModule.PageModuleTypeTemplate, command.PageModuleTypeTemplateId);
            }
            else if (command.PageModuleTypeTemplateId != dbModule.PageModuleTypeTemplateId)
            {
                dbModule.PageModuleTypeTemplate = null;
            }

            using (var scope = _transactionScopeFactory.Create())
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    PageVersionModuleEntityDefinition.DefinitionCode,
                    dbModule.PageVersionModuleId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);
                scope.Complete();
            }
            _pageCache.Clear(dbModule.PageVersion.PageId);

            await _messageAggregator.PublishAsync(new PageVersionModuleUpdatedMessage()
            {
                PageId = dbModule.PageVersion.PageId,
                PageVersionModuleId = command.PageVersionModuleId,
                HasPublishedVersionChanged = dbModule.PageVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageVersionModuleCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
