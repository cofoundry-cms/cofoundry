using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IOpenGraphDataMapper"/>.
/// </summary>
public class OpenGraphDataMapper : IOpenGraphDataMapper
{
    private readonly IImageAssetRenderDetailsMapper _imageAssetRenderDetailsMapper;

    public OpenGraphDataMapper(
        IImageAssetRenderDetailsMapper imageAssetRenderDetailsMapper
        )
    {
        _imageAssetRenderDetailsMapper = imageAssetRenderDetailsMapper;
    }

    /// <inheritdoc/>
    public virtual OpenGraphData Map(PageVersion dbPageVersion)
    {
        var result = new OpenGraphData()
        {
            Description = dbPageVersion.OpenGraphDescription,
            Title = dbPageVersion.OpenGraphTitle
        };

        MissingIncludeException.ThrowIfNull(dbPageVersion, v => v.OpenGraphImageAsset, v => v.OpenGraphImageId);

        if (dbPageVersion.OpenGraphImageAsset != null)
        {
            result.Image = _imageAssetRenderDetailsMapper.Map(dbPageVersion.OpenGraphImageAsset);
        }

        return result;
    }
}
