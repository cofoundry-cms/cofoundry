using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageDetailsByIdQuery : IQuery<PageDetails>
    {
        public GetPageDetailsByIdQuery()
        {
        }

        public GetPageDetailsByIdQuery(int pageId)
        {
            PageId = pageId;
        }

        public int PageId { get; set; }
    }
}
