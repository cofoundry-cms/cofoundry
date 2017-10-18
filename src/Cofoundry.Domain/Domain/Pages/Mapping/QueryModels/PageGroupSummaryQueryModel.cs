using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.QueryModels
{
    public class PageGroupSummaryQueryModel
    {
        public PageGroup PageGroup { get; set; }

        public User Creator { get; set; }

        public int NumPages { get; set; }
    }
}
