using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetImageAssetEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetImageAssetEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetImageAssetEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            Condition.Requires(ids).IsNotNull();

            ImageAssetIds = ids.ToArray();
        }

        [Required]
        public int[] ImageAssetIds { get; set; }
    }
}
