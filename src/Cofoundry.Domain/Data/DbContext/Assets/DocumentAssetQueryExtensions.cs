namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{DocumentAsset}"/>.
/// </summary>
public static class DocumentAssetQueryExtensions
{
    extension(IQueryable<DocumentAsset> documents)
    {
        public IQueryable<DocumentAsset> FilterById(int id)
        {
            var result = documents
                .Where(i => i.DocumentAssetId == id);

            return result;
        }
    }

    extension(IQueryable<DocumentAsset> document)
    {
        public IQueryable<DocumentAsset> FilterByIds(IEnumerable<int> ids)
        {
            var result = document
                .Where(i => ids.Contains(i.DocumentAssetId));

            return result;
        }
    }
}
