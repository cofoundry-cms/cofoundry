using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IImageAssetSummaryMapper"/>.
/// </summary>
public class ImageAssetSummaryMapper : IImageAssetSummaryMapper
{
    private readonly IAuditDataMapper _auditDataMapper;
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

    public ImageAssetSummaryMapper(
        IAuditDataMapper auditDataMapper,
        IImageAssetRouteLibrary imageAssetRouteLibrary
        )
    {
        _auditDataMapper = auditDataMapper;
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbImage))]
    public ImageAssetSummary? Map(ImageAsset? dbImage)
    {
        if (dbImage == null)
        {
            return null;
        }

        var image = new ImageAssetSummary();
        Map(image, dbImage);

        return image;
    }

    /// <summary>
    /// Used internally to map a model that inherits from ImageAssetSummary. 
    /// </summary>
    public ImageAssetSummary Map<TModel>(TModel imageToMap, ImageAsset dbImage)
        where TModel : ImageAssetSummary
    {
        imageToMap.AuditData = _auditDataMapper.MapUpdateAuditData(dbImage);
        imageToMap.ImageAssetId = dbImage.ImageAssetId;
        imageToMap.FileExtension = dbImage.FileExtension;
        imageToMap.FileName = dbImage.FileName;
        imageToMap.FileSizeInBytes = dbImage.FileSizeInBytes;
        imageToMap.Height = dbImage.HeightInPixels;
        imageToMap.Width = dbImage.WidthInPixels;
        imageToMap.Title = dbImage.Title;
        imageToMap.FileStamp = AssetFileStampHelper.ToFileStamp(dbImage.FileUpdateDate);
        imageToMap.FileNameOnDisk = dbImage.FileNameOnDisk;
        imageToMap.DefaultAnchorLocation = dbImage.DefaultAnchorLocation;
        imageToMap.VerificationToken = dbImage.VerificationToken;

        imageToMap.Tags = dbImage
            .ImageAssetTags
            .Select(t => t.Tag.TagText)
            .OrderBy(t => t)
            .ToArray();

        imageToMap.Url = _imageAssetRouteLibrary.ImageAsset(imageToMap);

        return imageToMap;
    }
}
