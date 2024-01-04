using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to <see cref="ImageAssetDetails"/> models.
/// </summary>
public interface IImageAssetDetailsMapper
{
    /// <summary>
    /// Maps an EF <see cref="ImageAsset"/> record from the db into an 
    /// <see cref="ImageAssetDetails"/> model. If the db record is 
    /// <see langword="null"/> then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="dbImage">Database record to map from.</param>
    [return: NotNullIfNotNull(nameof(dbImage))]
    ImageAssetDetails? Map(ImageAsset? dbImage);
}
