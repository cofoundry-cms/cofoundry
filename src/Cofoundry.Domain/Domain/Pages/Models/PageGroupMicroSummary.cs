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
    public class PageGroupMicroSummary
    {
        public int PageGroupId { get; set; }

        public int? ParentGroupId { get; set; }

        public string Name { get; set; }
    }
}
