using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Encapsulates routing information from a traditional PageRoute as
    /// well as a CustomEntityRoute if it is available.
    /// </summary>
    public class PageRoutingInfo
    {
        public PageRoute PageRoute { get; set; }

        public CustomEntityRoute CustomEntityRoute { get; set; }

        public ICustomEntityRoutingRule CustomEntityRouteRule { get; set; }

        /// <summary>
        /// Gets an IVersionRoute that matches the specified workFlowStatusQuery and version number.
        /// </summary>
        /// <param name="preferCustomEntity">Look for the CustomEntityRouting if its available.</param>
        public IVersionRoute GetVersionRoute(bool preferCustomEntity, WorkFlowStatusQuery workFlowStatusQuery, int? versionId)
        {
            IEnumerable<IVersionRoute> versions = null;

            if (preferCustomEntity && CustomEntityRoute != null)
            {
                versions = CustomEntityRoute.Versions;
            }
            else if (PageRoute != null)
            {
                versions = PageRoute.Versions;
            }

            if (versions != null)
            {
                return versions.GetVersionRouting(workFlowStatusQuery, versionId);
            }

            return null;
        }
    }
}
