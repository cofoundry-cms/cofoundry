using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Primarily used in the admin area, the PageDetails object includes
    /// audit data and other additional information that should normally be 
    /// hidden from a customer facing app.
    /// </summary>
    public class PageDetails : ICreateAudited
    {
        public int PageId { get; set; }

        public PageRoute PageRoute { get; set; }

        /// <summary>
        /// These tags are used in the admin panel for searching and categorizing.
        /// </summary>
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// Data for the latest version of the page, which is not
        /// neccessarily published.
        /// </summary>
        public PageVersionDetails LatestVersion { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
