using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByPageDirectoryIdQuery : IQuery<IEnumerable<PageRoute>>
    {
        public GetPageRoutesByPageDirectoryIdQuery() { }

        public GetPageRoutesByPageDirectoryIdQuery(int id)
        {
            PageDirectoryId = id;
        }

        public int PageDirectoryId { get; set; }
    }
}
