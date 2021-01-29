using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a new image asset.
    /// </summary>
    public class AddImageAssetCommandHandler 
        : ICommandHandler<AddImageAssetCommand>
        , IPermissionRestrictedCommandHandler<AddImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IImageAssetFileService _imageAssetFileService;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IAssetFileTypeValidator _assetFileTypeValidator;

        public AddImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IImageAssetFileService imageAssetFileService,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator,
            IRandomStringGenerator randomStringGenerator,
            IAssetFileTypeValidator assetFileTypeValidator
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _imageAssetFileService = imageAssetFileService;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
            _randomStringGenerator = randomStringGenerator;
            _assetFileTypeValidator = assetFileTypeValidator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(AddImageAssetCommand command, IExecutionContext executionContext)
        {
            var imageAsset = new ImageAsset();

            imageAsset.Title = command.Title;
            imageAsset.FileName = SlugFormatter.ToSlug(command.Title);
            imageAsset.DefaultAnchorLocation = command.DefaultAnchorLocation;
            imageAsset.FileUpdateDate = executionContext.ExecutionDate;
            imageAsset.FileNameOnDisk = "file-not-saved";
            imageAsset.FileExtension = "unknown";
            imageAsset.VerificationToken = _randomStringGenerator.Generate(6);

            _assetFileTypeValidator.ValidateAndThrow(command.File.FileName, command.File.MimeType, nameof(command.File));

            var fileStamp = AssetFileStampHelper.ToFileStamp(imageAsset.FileUpdateDate);

            _entityTagHelper.UpdateTags(imageAsset.ImageAssetTags, command.Tags, executionContext);
            _entityAuditHelper.SetCreated(imageAsset, executionContext);

            _dbContext.ImageAssets.Add(imageAsset);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                // Save first to get an Id
                await _dbContext.SaveChangesAsync();

                // Update the disk filename
                imageAsset.FileNameOnDisk = $"{imageAsset.ImageAssetId}-{fileStamp}";

                await _imageAssetFileService.SaveAsync(command.File, imageAsset, nameof(command.File));

                command.OutputImageAssetId = imageAsset.ImageAssetId;

                scope.QueueCompletionTask(() => OnTransactionComplete(imageAsset));

                await scope.CompleteAsync();
            }
        }

        private Task OnTransactionComplete(ImageAsset imageAsset)
        {
            return _messageAggregator.PublishAsync(new ImageAssetAddedMessage()
            {
                ImageAssetId = imageAsset.ImageAssetId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddImageAssetCommand command)
        {
            yield return new ImageAssetCreatePermission();
        }

        #endregion
    }
}
