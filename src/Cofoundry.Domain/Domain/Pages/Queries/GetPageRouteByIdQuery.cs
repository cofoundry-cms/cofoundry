using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageRouteByIdQuery : IQuery<PageRoute>
    {
        public GetPageRouteByIdQuery()
        {
        }

        public GetPageRouteByIdQuery(int pageId)
        {
            PageId = pageId;
        }

        public int PageId { get; set; }
    }
}
