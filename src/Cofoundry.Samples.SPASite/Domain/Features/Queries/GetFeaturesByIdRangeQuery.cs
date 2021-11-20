using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetFeaturesByIdRangeQuery : IQuery<IDictionary<int, Feature>>
    {
        public GetFeaturesByIdRangeQuery() { }

        public GetFeaturesByIdRangeQuery(ICollection<int> ids)
        {
            FeatureIds = ids;
        }

        public ICollection<int> FeatureIds { get; set; }
    }
}
