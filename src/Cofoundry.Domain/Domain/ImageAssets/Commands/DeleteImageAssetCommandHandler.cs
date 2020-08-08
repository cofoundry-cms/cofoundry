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
    /// <summary>
    /// Removes an image asset from the system and
    /// queues any related files or caches to be removed
    /// as a separate process.
    /// </summary>
    public class DeleteImageAssetCommandHandler 
        : ICommandHandler<DeleteImageAssetCommand>
        , IPermissionRestrictedCommandHandler<DeleteImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IFileStoreService _fileStoreService;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;

        public DeleteImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache,
            ICommandExecutor commandExecutor,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator,
            IFileStoreService fileStoreService,
            IResizedImageAssetFileService resizedImageAssetFileService
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
            _fileStoreService = fileStoreService;
            _resizedImageAssetFileService = resizedImageAssetFileService;
        }

        #endregion

        public async Task ExecuteAsync(DeleteImageAssetCommand command, IExecutionContext executionContext)
        {
            var imageAsset = await _dbContext
                .ImageAssets
                .FilterById(command.ImageAssetId)
                .SingleOrDefaultAsync();

            if (imageAsset != null)
            {
                _dbContext.ImageAssets.Remove(imageAsset);

                var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, imageAsset.FileExtension);
                var deleteUnstructuredDataComand = new DeleteUnstructuredDataDependenciesCommand(ImageAssetEntityDefinition.DefinitionCode, imageAsset.ImageAssetId);
                var deleteFileCommand = new QueueAssetFileDeletionCommand()
                {
                    EntityDefinitionCode = ImageAssetEntityDefinition.DefinitionCode,
                    FileNameOnDisk = imageAsset.FileNameOnDisk,
                    FileExtension = imageAsset.FileExtension
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

        private Task OnTransactionComplete(DeleteImageAssetCommand command)
        {
            _imageAssetCache.Clear(command.ImageAssetId);

            return _messageAggregator.PublishAsync(new ImageAssetDeletedMessage()
            {
                ImageAssetId = command.ImageAssetId
            });
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteImageAssetCommand command)
        {
            yield return new ImageAssetDeletePermission();
        }

        #endregion
    }
}
