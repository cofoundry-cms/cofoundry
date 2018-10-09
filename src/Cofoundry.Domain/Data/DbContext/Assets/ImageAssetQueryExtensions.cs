using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class ImageAssetQueryExtensions
    {
        public static IQueryable<ImageAsset> FilterById(this IQueryable<ImageAsset> images, int id)
        {
            var result = images
                .Where(i => i.ImageAssetId == id);

            return result;
        }

        public static IQueryable<ImageAsset> FilterByIds(this IQueryable<ImageAsset> images, IEnumerable<int> ids)
        {
            var result = images
                .Where(i => ids.Contains(i.ImageAssetId));

            return result;
        }
    }
}
