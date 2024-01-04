using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to ImageAssetRenderDetails objects.
/// </summary>
public interface IImageAssetRenderDetailsMapper
{
    /// <summary>
    /// Maps an EF <see cref="ImageAsset"/> record from the db into an 
    /// <see cref="ImageAssetRenderDetails"/> model. If the db record is 
    /// <see langword="null"/> then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="dbImage"><see cref="ImageAsset"/> record from the database.</param>
    [return: NotNullIfNotNull(nameof(dbImage))]
    ImageAssetRenderDetails? Map(ImageAsset? dbImage);
}
