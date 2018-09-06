using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to describe an object that contains dynamic page routing
    /// information. Currently applied to a PageRoute and a CustomEntityRoute.
    /// </summary>
    public interface IVersionRoute
    {
        /// <summary>
        /// The database identifier for this route version. The data
        /// used for this property depends on the implementation.
        /// </summary>
        int VersionId { get; set; }

        /// <summary>
        /// The workflow state of this version e.g. draft/published.
        /// </summary>
        WorkFlowStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// A user friendly title of the version.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// A page can have many published versions, this flag indicates if
        /// it is the latest published version which displays on the live site
        /// when the page itself is published.
        /// </summary>
        bool IsLatestPublishedVersion { get; set; }

        /// <summary>
        /// Date that the version was created.
        /// </summary>
        DateTime CreateDate { get; set; }
    }
}
