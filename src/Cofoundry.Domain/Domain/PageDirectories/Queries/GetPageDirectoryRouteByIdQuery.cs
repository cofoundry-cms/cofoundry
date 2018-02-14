using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageDirectoryRouteByIdQuery : IQuery<PageDirectoryRoute>
    {
        public GetPageDirectoryRouteByIdQuery()
        {
        }

        public GetPageDirectoryRouteByIdQuery(int pageDirectoryId)
        {
            PageDirectoryId = pageDirectoryId;
        }

        public int PageDirectoryId { get; set; }
    }
}
