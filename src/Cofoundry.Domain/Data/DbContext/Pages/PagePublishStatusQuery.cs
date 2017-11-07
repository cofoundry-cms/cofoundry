using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PagePublishStatusQuery
    {
        public int PageId { get; set; }

        public short PublishStatusQueryId { get; set; }

        public int PageVersionId { get; set; }

        public virtual Page Page { get; set; }

        public virtual PageVersion PageVersion { get; set; }

    }
}
