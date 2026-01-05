namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{ImageAsset}"/>.
/// </summary>
public static class ImageAssetQueryExtensions
{
    extension(IQueryable<ImageAsset> images)
    {
        public IQueryable<ImageAsset> FilterById(int id)
        {
            var result = images
                .Where(i => i.ImageAssetId == id);

            return result;
        }

        public IQueryable<ImageAsset> FilterByIds(IEnumerable<int> ids)
        {
            var result = images
                .Where(i => ids.Contains(i.ImageAssetId));

            return result;
        }
    }
}
