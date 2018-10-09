using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class DocumentAssetQueryExtensions
    {
        public static IQueryable<DocumentAsset> FilterById(this IQueryable<DocumentAsset> documents, int id)
        {
            var result = documents
                .Where(i => i.DocumentAssetId == id);

            return result;
        }

        public static IQueryable<DocumentAsset> FilterByIds(this IQueryable<DocumentAsset> document, IEnumerable<int> ids)
        {
            var result = document
                .Where(i => ids.Contains(i.DocumentAssetId));

            return result;
        }
    }
}
