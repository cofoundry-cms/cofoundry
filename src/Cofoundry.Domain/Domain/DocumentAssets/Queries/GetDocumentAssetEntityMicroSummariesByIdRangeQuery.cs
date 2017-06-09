using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetDocumentAssetEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetDocumentAssetEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            DocumentAssetIds = ids.ToArray();
        }

        [Required]
        public int[] DocumentAssetIds { get; set; }
    }
}
