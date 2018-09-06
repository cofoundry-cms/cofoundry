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
        /// <summary>
        /// Database id of the page record.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The routing data for the page.
        /// </summary>
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

        /// <summary>
        /// Simple audit data for page creation.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
