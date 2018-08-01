using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents routing information for a specific version of
    /// a custom entity. Cached as part of a CustomEntityRoute.
    /// </summary>
    public class CustomEntityVersionRoute : IVersionRoute
    {
        public CustomEntityVersionRoute()
        {
            AdditionalRoutingData = new Dictionary<string, string>();
        }

        /// <summary>
        /// The database identifier for this route version required by
        /// the IVersionRoute. In this case this is the CustomEntityVersionId.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        /// The workflow state of this version e.g. draft/published.
        /// </summary>
        public WorkFlowStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// The title of the custom entity.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A page can have many published versions, this flag indicates if
        /// it is the latest published version which displays on the live site
        /// when the page itself is published.
        /// </summary>
        public bool IsLatestPublishedVersion { get; set; }

        /// <summary>
        /// Date the custom entity was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Used to store any additonal parameters required for routing
        /// to this entity. To add data to this collection you need to 
        /// annotate the property with CustomEntityRouteDataAttribute. 
        /// </summary>
        public Dictionary<string, string> AdditionalRoutingData { get; set; }
    }
}
