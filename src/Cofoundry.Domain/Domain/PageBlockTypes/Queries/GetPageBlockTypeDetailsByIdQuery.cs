using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a block type by it's unique database id.
    /// </summary>
    public class GetPageBlockTypeDetailsByIdQuery : IQuery<PageBlockTypeDetails>
    {
        public GetPageBlockTypeDetailsByIdQuery()
        {
        }

        public GetPageBlockTypeDetailsByIdQuery(int pageBlockTypeId)
        {
            PageBlockTypeId = pageBlockTypeId;
        }

        public int PageBlockTypeId { get; set; }
    }
}
