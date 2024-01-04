using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IImageAssetRenderDetailsMapper"/>.
/// </summary>
public class ImageAssetRenderDetailsMapper : IImageAssetRenderDetailsMapper
{
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

    public ImageAssetRenderDetailsMapper(
        IImageAssetRouteLibrary imageAssetRouteLibrary
        )
    {
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbImage))]
    public ImageAssetRenderDetails? Map(ImageAsset? dbImage)
    {
        if (dbImage == null)
        {
            return null;
        }

        var image = new ImageAssetRenderDetails()
        {
            ImageAssetId = dbImage.ImageAssetId,
            FileExtension = dbImage.FileExtension,
            FileName = dbImage.FileName,
            FileNameOnDisk = dbImage.FileNameOnDisk,
            Height = dbImage.HeightInPixels,
            Width = dbImage.WidthInPixels,
            Title = dbImage.Title,
            DefaultAnchorLocation = dbImage.DefaultAnchorLocation,
            FileUpdateDate = dbImage.FileUpdateDate,
            VerificationToken = dbImage.VerificationToken
        };

        image.FileStamp = AssetFileStampHelper.ToFileStamp(dbImage.FileUpdateDate);
        image.Url = _imageAssetRouteLibrary.ImageAsset(image);

        return image;
    }
}
