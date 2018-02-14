using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageVersionSummariesByPageIdQuery : IQuery<ICollection<PageVersionSummary>>
    {
        public GetPageVersionSummariesByPageIdQuery()
        {
        }

        public GetPageVersionSummariesByPageIdQuery(int pageId)
        {
            PageId = pageId;
        }

        public int PageId { get; set; }
    }
}
