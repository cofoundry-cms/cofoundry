using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByIdRangeQuery : IQuery<IDictionary<int, PageRoute>>
    {
        public GetPageRoutesByIdRangeQuery() { }
        
        public GetPageRoutesByIdRangeQuery(IEnumerable<int> pageIds)
            : this(pageIds?.ToList())
        {
        }
        
        public GetPageRoutesByIdRangeQuery(IReadOnlyCollection<int> pageIds)
        {
            PageIds = pageIds;
        }

        public IReadOnlyCollection<int> PageIds { get; set; }
    }
}
