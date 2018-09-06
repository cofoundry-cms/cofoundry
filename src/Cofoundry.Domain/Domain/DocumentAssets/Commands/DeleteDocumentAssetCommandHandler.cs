using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain
{
    public class DeleteDocumentAssetCommandHandler 
        : IAsyncCommandHandler<DeleteDocumentAssetCommand>
        , IPermissionRestrictedCommandHandler<DeleteDocumentAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;

        public DeleteDocumentAssetCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(DeleteDocumentAssetCommand command, IExecutionContext executionContext)
        {
            var documentAsset = await _dbContext
                .DocumentAssets
                .FilterById(command.DocumentAssetId)
                .SingleOrDefaultAsync();

            if (documentAsset != null)
            {
                documentAsset.IsDeleted = true;
                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(DocumentAssetEntityDefinition.DefinitionCode, documentAsset.DocumentAssetId), executionContext);

                    await _dbContext.SaveChangesAsync();

                    scope.QueueCompletionTask(() => OnTransactionComplete(command));

                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(DeleteDocumentAssetCommand command)
        {
            return _messageAggregator.PublishAsync(new DocumentAssetAddedMessage()
            {
                DocumentAssetId = command.DocumentAssetId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteDocumentAssetCommand command)
        {
            yield return new DocumentAssetDeletePermission();
        }

        #endregion
    }
}
