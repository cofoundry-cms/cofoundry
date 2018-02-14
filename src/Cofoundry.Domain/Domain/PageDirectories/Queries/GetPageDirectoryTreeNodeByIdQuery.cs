using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetPageDirectoryNodeByIdQuery : IQuery<PageDirectoryNode>
    {
        public GetPageDirectoryNodeByIdQuery()
        {
        }

        public GetPageDirectoryNodeByIdQuery(int pageDirectoryId)
        {
            PageDirectoryId = pageDirectoryId;
        }

        public int PageDirectoryId { get; set; }
    }
}
