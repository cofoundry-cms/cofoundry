using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class WorkFlowStatus
    {
        public WorkFlowStatus()
        {
            PageVersions = new List<PageVersion>();
        }

        public int WorkFlowStatusId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PageVersion> PageVersions { get; set; }
    }
}
