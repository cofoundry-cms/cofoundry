using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetImageAssetRenderDetailsByIdRangeQuery : IQuery<IDictionary<int, ImageAssetRenderDetails>>
    {
        public GetImageAssetRenderDetailsByIdRangeQuery()
        {
        }

        public GetImageAssetRenderDetailsByIdRangeQuery(
            IEnumerable<int> ids
            )
            : this(ids?.ToList())
        {
        }

        public GetImageAssetRenderDetailsByIdRangeQuery(
            IReadOnlyCollection<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            ImageAssetIds = ids;
        }

        [Required]
        public IReadOnlyCollection<int> ImageAssetIds { get; set; }
    }
}
