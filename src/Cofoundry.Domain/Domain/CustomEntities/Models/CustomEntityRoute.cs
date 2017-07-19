using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains a small amount of custom entity identity and
    /// page routing information. These route objects are cached 
    /// in order to make routing lookups speedy.
    /// </summary>
    public class CustomEntityRoute
    {
        /// <summary>
        /// Unique 6 letter code representing the type of custom entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Optional locale assigned to the custom entity
        /// if used in a localized site.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// The unique string identifier slug which can
        /// be used in the routing of the custom entity page.
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// Routing information particular to specific versions.
        /// </summary>
        public IEnumerable<CustomEntityVersionRoute> Versions { get; set; }
    }
}
