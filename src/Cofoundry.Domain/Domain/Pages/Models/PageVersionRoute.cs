using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents routing information for a specific version of
    /// a page. Cached as part of a PageRoute.
    /// </summary>
    public class PageVersionRoute : IVersionRoute
    {
        /// <summary>
        /// The database identifier for this route version required by
        /// the IVersionRoute. In this case this is the PageVersionId.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        /// The workflow state of this version e.g. draft/published.
        /// </summary>
        public WorkFlowStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// The display title of the page. Often used as the meta
        /// title of the webpage, but it is not mandatory.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A page can have many published versions, this flag indicates if
        /// it is the latest published version which displays on the live site
        /// when the page itself is published.
        /// </summary>
        public bool IsLatestPublishedVersion { get; set; }

        /// <summary>
        /// Date that the page version was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Database id of the template implemented by this version.
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// True if the template implemented by this version has customizable
        /// regions for page data defined; otherwise false.
        /// </summary>
        public bool HasPageRegions { get; set; }

        /// <summary>
        /// True if the template implemented by this version has customizable
        /// regions for custom entity data defined; otherwise false.
        /// </summary>
        public bool HasCustomEntityRegions { get; set; }
    }
}
