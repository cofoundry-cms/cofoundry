using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IImageAssetDetailsMapper"/>.
/// </summary>
public class ImageAssetDetailsMapper : IImageAssetDetailsMapper
{
    private readonly ImageAssetSummaryMapper _imageAssetSummaryMapper;

    public ImageAssetDetailsMapper(
        IAuditDataMapper auditDataMapper,
        IImageAssetRouteLibrary imageAssetRouteLibrary
        )
    {
        _imageAssetSummaryMapper = new ImageAssetSummaryMapper(auditDataMapper, imageAssetRouteLibrary);
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbImage))]
    public ImageAssetDetails? Map(ImageAsset? dbImage)
    {
        if (dbImage == null)
        {
            return null;
        }

        var image = new ImageAssetDetails();
        _imageAssetSummaryMapper.Map(image, dbImage);
        image.FileUpdateDate = dbImage.FileUpdateDate;

        return image;
    }
}
