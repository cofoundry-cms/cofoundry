using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the properties of an existing image asset. Updating
    /// the file is optional, but if you do then existing links to the
    /// asset file will redirect to the new asset file.
    /// </summary>
    public class UpdateImageAssetCommandHandler 
        : ICommandHandler<UpdateImageAssetCommand>
        , IPermissionRestrictedCommandHandler<UpdateImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IImageAssetFileService _imageAssetFileService;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly IResizedImageAssetFileService _imageAssetFileCache;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IAssetFileTypeValidator _assetFileTypeValidator;

        public UpdateImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IImageAssetFileService imageAssetFileService,
            IImageAssetCache imageAssetCache,
            IResizedImageAssetFileService imageAssetFileCache,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator,
            ICommandExecutor commandExecutor,
            IAssetFileTypeValidator assetFileTypeValidator
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _imageAssetFileService = imageAssetFileService;
            _imageAssetCache = imageAssetCache;
            _imageAssetFileCache = imageAssetFileCache;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
            _commandExecutor = commandExecutor;
            _assetFileTypeValidator = assetFileTypeValidator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(UpdateImageAssetCommand command, IExecutionContext executionContext)
        {
            bool hasNewFile = command.File != null;

            if (hasNewFile)
            {
                _assetFileTypeValidator.ValidateAndThrow(command.File.FileName, command.File.MimeType, nameof(command.File));
            }

            var imageAsset = await _dbContext
                .ImageAssets
                .Include(a => a.ImageAssetTags)
                .ThenInclude(a => a.Tag)
                .FilterById(command.ImageAssetId)
                .SingleOrDefaultAsync();

            imageAsset.Title = command.Title;
            imageAsset.FileName = SlugFormatter.ToSlug(command.Title);
            imageAsset.DefaultAnchorLocation = command.DefaultAnchorLocation;

            _entityTagHelper.UpdateTags(imageAsset.ImageAssetTags, command.Tags, executionContext);
            _entityAuditHelper.SetUpdated(imageAsset, executionContext);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                if (hasNewFile)
                {
                    var deleteOldFileCommand = new QueueAssetFileDeletionCommand()
                    {
                        EntityDefinitionCode = ImageAssetEntityDefinition.DefinitionCode,
                        FileNameOnDisk = imageAsset.FileNameOnDisk,
                        FileExtension = imageAsset.FileExtension
                    };

                    imageAsset.FileUpdateDate = executionContext.ExecutionDate;
                    var fileStamp = AssetFileStampHelper.ToFileStamp(imageAsset.FileUpdateDate);
                    imageAsset.FileNameOnDisk = $"{imageAsset.ImageAssetId}-{fileStamp}";

                    await _commandExecutor.ExecuteAsync(deleteOldFileCommand);
                    await _imageAssetFileService.SaveAsync(command.File, imageAsset, nameof(command.File));
                }

                await _dbContext.SaveChangesAsync();

                scope.QueueCompletionTask(() => OnTransactionComplete(hasNewFile, imageAsset));

                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(bool hasNewFile, ImageAsset imageAsset)
        {
            if (hasNewFile)
            {
                await _imageAssetFileCache.ClearAsync(imageAsset.FileNameOnDisk);
            }

            _imageAssetCache.Clear(imageAsset.ImageAssetId);

            await _messageAggregator.PublishAsync(new ImageAssetUpdatedMessage()
            {
                ImageAssetId = imageAsset.ImageAssetId,
                HasFileChanged = hasNewFile
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateImageAssetCommand command)
        {
            yield return new ImageAssetUpdatePermission();
        }

        #endregion
    }
}
