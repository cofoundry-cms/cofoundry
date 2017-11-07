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
        /// Gets an IVersionRoute that matches the specified publishStatusQuery and version number.
        /// </summary>
        /// <param name="preferCustomEntity">Look for the CustomEntityRouting if its available.</param>
        /// <param name="publishStatusQuery">Specifies how to query for the version e.g. prefer publishes or draft version.</param>
        /// <param name="versionId">Id of a specifc version to look for if using PublishStatusQuery.SpecificVersion.</param>
        public IVersionRoute GetVersionRoute(bool preferCustomEntity, PublishStatusQuery publishStatusQuery, int? versionId)
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
                return versions.GetVersionRouting(publishStatusQuery, versionId);
            }

            return null;
        }
    }
}
