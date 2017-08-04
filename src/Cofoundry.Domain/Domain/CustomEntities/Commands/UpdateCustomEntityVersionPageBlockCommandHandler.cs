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
    public class UpdateCustomEntityVersionPageBlockCommandHandler
        : IAsyncCommandHandler<UpdateCustomEntityVersionPageBlockCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IPageBlockCommandHelper _pageBlockCommandHelper;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UpdateCustomEntityVersionPageBlockCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            ICustomEntityCache pageCache,
            IPageBlockCommandHelper pageBlockCommandHelper,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _customEntityCache = pageCache;
            _pageBlockCommandHelper = pageBlockCommandHelper;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext)
        {
            var dbBlock = await _dbContext
                .CustomEntityVersionPageBlocks
                .Include(m => m.PageBlockTypeTemplate)
                .Include(m => m.PageBlockType)
                .Include(m => m.CustomEntityVersion)
                .ThenInclude(v => v.CustomEntity)
                .Where(l => l.CustomEntityVersionPageBlockId == command.CustomEntityVersionPageBlockId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbBlock, command.CustomEntityVersionPageBlockId);
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityUpdatePermission>(dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode);

            if (dbBlock.CustomEntityVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be updated unless the entity is in draft status");
            }

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _pageBlockCommandHelper.UpdateModelAsync(command, dbBlock);
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    CustomEntityVersionPageBlockEntityDefinition.DefinitionCode,
                    dbBlock.CustomEntityVersionPageBlockId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand);

                scope.Complete();
            }

            _customEntityCache.Clear(dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode, dbBlock.CustomEntityVersion.CustomEntity.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityVersionBlockUpdatedMessage()
            {
                CustomEntityId = dbBlock.CustomEntityVersion.CustomEntityId,
                CustomEntityDefinitionCode = dbBlock.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityVersionBlockId = dbBlock.CustomEntityVersionPageBlockId
            });
        }

        #endregion
    }
}
