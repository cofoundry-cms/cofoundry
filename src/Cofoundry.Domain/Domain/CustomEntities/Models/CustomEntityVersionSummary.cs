using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionSummary : ICreateAudited
    {
        public int CustomEntityVersionId { get; set; }

        public string Title { get; set; }
                
        public WorkFlowStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// A page can have many published versions, this flag indicates if
        /// it is the latest published version which displays on the live site
        /// when the page itself is published.
        /// </summary>
        public bool IsLatestPublishedVersion { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
