using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageBlockTypeSummaryByIdQuery : IQuery<PageBlockTypeSummary>
    {
        public GetPageBlockTypeSummaryByIdQuery()
        {
        }

        public GetPageBlockTypeSummaryByIdQuery(int pageBlockTypeId)
        {
            PageBlockTypeId = pageBlockTypeId;
        }

        public int PageBlockTypeId { get; set; }
    }
}
