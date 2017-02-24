using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByWebDirectoryIdQuery : IQuery<IEnumerable<PageRoute>>
    {
        public GetPageRoutesByWebDirectoryIdQuery() { }

        public GetPageRoutesByWebDirectoryIdQuery(int id)
        {
            WebDirectoryId = id;
        }

        public int WebDirectoryId { get; set; }
    }
}
