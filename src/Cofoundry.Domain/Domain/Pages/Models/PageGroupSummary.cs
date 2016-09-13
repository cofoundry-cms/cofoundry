using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A page group is a categorisation of a page that can be used to provide custom functionality
    /// </summary>
    /// <remarks>
    /// Not yet implemented in the admin UI, needs to be thought about and re-implemented. Could be useful
    /// for working with selections of pages e.g. when building a menu, but should probably have a text identifier
    /// </remarks>
    public class PageGroupSummary : ICreateAudited
    {
        public int PageGroupId { get; set; }

        public int? ParentGroupId { get; set; }

        public string Name { get; set; }

        public int NumPages { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
