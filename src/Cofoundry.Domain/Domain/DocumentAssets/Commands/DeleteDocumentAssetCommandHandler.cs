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
using System.IO;

namespace Cofoundry.Domain.Internal
{
    public class DeleteDocumentAssetCommandHandler 
        : ICommandHandler<DeleteDocumentAssetCommand>
        , IPermissionRestrictedCommandHandler<DeleteDocumentAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IFileStoreService _fileStoreService;

        public DeleteDocumentAssetCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator,
            IFileStoreService fileStoreService
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
            _fileStoreService = fileStoreService;
        }

        #endregion

        public async Task ExecuteAsync(DeleteDocumentAssetCommand command, IExecutionContext executionContext)
        {
            var documentAsset = await _dbContext
                .DocumentAssets
                .FilterById(command.DocumentAssetId)
                .SingleOrDefaultAsync();

            if (documentAsset != null)
            {
                _dbContext.DocumentAssets.Remove(documentAsset);

                var fileName = Path.ChangeExtension(documentAsset.FileNameOnDisk, documentAsset.FileExtension);
                var deleteUnstructuredDataComand = new DeleteUnstructuredDataDependenciesCommand(DocumentAssetEntityDefinition.DefinitionCode, documentAsset.DocumentAssetId);
                var deleteFileCommand = new QueueAssetFileDeletionCommand()
                {
                    EntityDefinitionCode = DocumentAssetEntityDefinition.DefinitionCode,
                    FileNameOnDisk = documentAsset.FileNameOnDisk,
                    FileExtension = documentAsset.FileExtension
                };

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _dbContext.SaveChangesAsync();
                    await _commandExecutor.ExecuteAsync(deleteFileCommand, executionContext);
                    await _commandExecutor.ExecuteAsync(deleteUnstructuredDataComand, executionContext);

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

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteDocumentAssetCommand command)
        {
            yield return new DocumentAssetDeletePermission();
        }

        #endregion
    }
}
