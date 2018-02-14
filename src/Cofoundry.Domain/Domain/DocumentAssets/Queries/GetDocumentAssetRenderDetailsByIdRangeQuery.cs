using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdRangeQuery : IQuery<IDictionary<int, DocumentAssetRenderDetails>>
    {
        public GetDocumentAssetRenderDetailsByIdRangeQuery()
        {
        }

        public GetDocumentAssetRenderDetailsByIdRangeQuery(
            IEnumerable<int> ids
            )
            : this(ids?.ToList())
        {
        }

        public GetDocumentAssetRenderDetailsByIdRangeQuery(
            IReadOnlyCollection<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            DocumentAssetIds = ids;
        }

        [Required]
        public IReadOnlyCollection<int> DocumentAssetIds { get; set; }
    }
}
