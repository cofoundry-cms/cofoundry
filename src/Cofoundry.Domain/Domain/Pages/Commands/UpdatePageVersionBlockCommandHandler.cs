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
    public class UpdatePageVersionBlockCommandHandler
        : ICommandHandler<UpdatePageVersionBlockCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageVersionBlockCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageVersionBlockCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            ICommandExecutor commandExecutor,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
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

        public async Task ExecuteAsync(UpdatePageVersionBlockCommand command, IExecutionContext executionContext)
        {
            var dbBlock = await _dbContext
                .PageVersionBlocks
                .Include(m => m.PageBlockTypeTemplate)
                .Include(m => m.PageBlockType)
                .Include(m => m.PageVersion)
                .Where(l => l.PageVersionBlockId == command.PageVersionBlockId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(dbBlock, command.PageVersionBlockId);

            if (dbBlock.PageVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft)
            {
                throw new NotPermittedException("Page blocks cannot be updated unless the page version is in draft status");
            }

            if (command.PageBlockTypeId != dbBlock.PageBlockTypeId)
            {
                var pageBlockType = await _dbContext
                    .PageBlockTypes
                    .Where(m => m.PageBlockTypeId == command.PageBlockTypeId)
                    .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(pageBlockType, command.PageBlockTypeId);
                dbBlock.PageBlockType = pageBlockType;
            }

            dbBlock.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.DataModel);
            dbBlock.UpdateDate = executionContext.ExecutionDate;

            if (command.PageBlockTypeTemplateId != dbBlock.PageBlockTypeTemplateId && command.PageBlockTypeTemplateId.HasValue)
            {
                dbBlock.PageBlockTypeTemplate = await _dbContext
                    .PageBlockTypeTemplates
                    .SingleOrDefaultAsync(m => m.PageBlockTypeId == dbBlock.PageBlockTypeId && m.PageBlockTypeTemplateId == command.PageBlockTypeTemplateId);
                EntityNotFoundException.ThrowIfNull(dbBlock.PageBlockTypeTemplate, command.PageBlockTypeTemplateId);
            }
            else if (command.PageBlockTypeTemplateId != dbBlock.PageBlockTypeTemplateId)
            {
                dbBlock.PageBlockTypeTemplate = null;
            }

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();

                var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                    PageVersionBlockEntityDefinition.DefinitionCode,
                    dbBlock.PageVersionBlockId,
                    command.DataModel);

                await _commandExecutor.ExecuteAsync(dependencyCommand, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(dbBlock));

                await scope.CompleteAsync();
            }
        }

        private Task OnTransactionComplete(PageVersionBlock dbBlock)
        {
            _pageCache.Clear(dbBlock.PageVersion.PageId);

            return _messageAggregator.PublishAsync(new PageVersionBlockUpdatedMessage()
            {
                PageId = dbBlock.PageVersion.PageId,
                PageVersionBlockId = dbBlock.PageVersionBlockId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageVersionBlockCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
