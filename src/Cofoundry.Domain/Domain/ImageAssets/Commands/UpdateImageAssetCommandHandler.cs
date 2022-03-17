using Cofoundry.Core.Data;
using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the properties of an existing image asset. Updating
/// the file is optional, but if you do then existing links to the
/// asset file will redirect to the new asset file.
/// </summary>
public class UpdateImageAssetCommandHandler
    : ICommandHandler<UpdateImageAssetCommand>
    , IPermissionRestrictedCommandHandler<UpdateImageAssetCommand>
{
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
    private readonly IMimeTypeService _mimeTypeService;

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
        IAssetFileTypeValidator assetFileTypeValidator,
        IMimeTypeService mimeTypeService
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
        _mimeTypeService = mimeTypeService;
    }

    public async Task ExecuteAsync(UpdateImageAssetCommand command, IExecutionContext executionContext)
    {
        bool hasNewFile = command.File != null;

        if (hasNewFile)
        {
            ValidateFileType(command);
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

    private void ValidateFileType(UpdateImageAssetCommand command)
    {
        var contentType = _mimeTypeService.MapFromFileName(command.File.FileName, command.File.MimeType);
        _assetFileTypeValidator.ValidateAndThrow(command.File.FileName, contentType, nameof(command.File));
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateImageAssetCommand command)
    {
        yield return new ImageAssetUpdatePermission();
    }
}
