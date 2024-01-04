namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to ImageAssetFile objects.
/// </summary>
public interface IImageAssetFileMapper
{
    /// <summary>
    /// Maps an <see cref="ImageAssetRenderDetails"/> model (which is 
    /// potentially cached) into an <see cref="ImageAssetFile"/> model. 
    /// If the <paramref name="renderDetails"/> model is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="renderDetails">Model to map from.</param>
    /// <param name="contentStream"><see cref="Stream"/> containing the image file data.</param>
    [return: NotNullIfNotNull(nameof(renderDetails))]
    ImageAssetFile? Map(ImageAssetRenderDetails? renderDetails, Stream contentStream);
}
