namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IImageAssetFileMapper"/>.
/// </summary>
public class ImageAssetFileMapper : IImageAssetFileMapper
{
    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(renderDetails))]
    public ImageAssetFile? Map(ImageAssetRenderDetails? renderDetails, Stream contentStream)
    {
        if (renderDetails == null)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(contentStream);

        var image = new ImageAssetFile()
        {
            ImageAssetId = renderDetails.ImageAssetId,
            FileExtension = renderDetails.FileExtension,
            FileName = renderDetails.FileName,
            FileNameOnDisk = renderDetails.FileNameOnDisk,
            Height = renderDetails.Height,
            Width = renderDetails.Width,
            Title = renderDetails.Title,
            DefaultAnchorLocation = renderDetails.DefaultAnchorLocation,
            FileStamp = renderDetails.FileStamp,
            FileUpdateDate = renderDetails.FileUpdateDate,
            ContentStream = contentStream,
            VerificationToken = renderDetails.VerificationToken
        };

        return image;
    }
}
