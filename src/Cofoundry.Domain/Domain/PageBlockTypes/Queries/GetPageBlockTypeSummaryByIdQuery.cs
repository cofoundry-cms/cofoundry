using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a block type by it's unique database id. The results of this query
    /// are cached by default.
    /// </summary>
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
