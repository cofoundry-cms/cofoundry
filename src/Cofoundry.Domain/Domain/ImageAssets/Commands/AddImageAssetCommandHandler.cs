using Cofoundry.Core.Data;
using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Adds a new image asset.
/// </summary>
public class AddImageAssetCommandHandler
    : ICommandHandler<AddImageAssetCommand>
    , IPermissionRestrictedCommandHandler<AddImageAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly EntityTagHelper _entityTagHelper;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IRandomStringGenerator _randomStringGenerator;
    private readonly IAssetFileTypeValidator _assetFileTypeValidator;
    private readonly IMimeTypeService _mimeTypeService;
    private readonly ImageAssetFileCommandHelper _imageAssetFileCommandHelper;

    public AddImageAssetCommandHandler(
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        EntityTagHelper entityTagHelper,
        ITransactionScopeManager transactionScopeManager,
        IMessageAggregator messageAggregator,
        IRandomStringGenerator randomStringGenerator,
        IAssetFileTypeValidator assetFileTypeValidator,
        IMimeTypeService mimeTypeService,
        ImageAssetFileCommandHelper imageAssetFileCommandHelper
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
        _entityTagHelper = entityTagHelper;
        _transactionScopeManager = transactionScopeManager;
        _messageAggregator = messageAggregator;
        _randomStringGenerator = randomStringGenerator;
        _assetFileTypeValidator = assetFileTypeValidator;
        _mimeTypeService = mimeTypeService;
        _imageAssetFileCommandHelper = imageAssetFileCommandHelper;
    }

    public async Task ExecuteAsync(AddImageAssetCommand command, IExecutionContext executionContext)
    {
        ValidateFileType(command);

        var imageAsset = new ImageAsset
        {
            Title = command.Title,
            FileName = SlugFormatter.ToSlug(command.Title),
            DefaultAnchorLocation = command.DefaultAnchorLocation,
            FileUpdateDate = executionContext.ExecutionDate,
            FileNameOnDisk = "file-not-saved",
            FileExtension = "unknown",
            VerificationToken = _randomStringGenerator.Generate(6)
        };

        var fileStamp = AssetFileStampHelper.ToFileStamp(imageAsset.FileUpdateDate);

        _entityTagHelper.UpdateTags(imageAsset.ImageAssetTags, command.Tags, executionContext);
        _entityAuditHelper.SetCreated(imageAsset, executionContext);

        _dbContext.ImageAssets.Add(imageAsset);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            // Save first to get an Id
            await _dbContext.SaveChangesAsync();

            // Update the disk filename
            imageAsset.FileNameOnDisk = $"{imageAsset.ImageAssetId}-{fileStamp}";

            await _imageAssetFileCommandHelper.SaveFileAsync(command.File, imageAsset, nameof(command.File));

            command.OutputImageAssetId = imageAsset.ImageAssetId;

            scope.QueueCompletionTask(() => OnTransactionComplete(imageAsset));

            await scope.CompleteAsync();
        }
    }

    private void ValidateFileType(AddImageAssetCommand command)
    {
        var contentType = _mimeTypeService.MapFromFileName(command.File.FileName, command.File.MimeType);
        _assetFileTypeValidator.ValidateAndThrow(command.File.FileName, contentType, nameof(command.File));
    }

    private Task OnTransactionComplete(ImageAsset imageAsset)
    {
        return _messageAggregator.PublishAsync(new ImageAssetAddedMessage()
        {
            ImageAssetId = imageAsset.ImageAssetId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddImageAssetCommand command)
    {
        yield return new ImageAssetCreatePermission();
    }
}
