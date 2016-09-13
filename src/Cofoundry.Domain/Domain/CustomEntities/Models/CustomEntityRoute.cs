using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityRoute
    {
        public int CustomEntityId { get; set; }

        public ActiveLocale Locale { get; set; }

        public string UrlSlug { get; set; }

        /// <summary>
        /// Routing information particular to specific versions.
        /// </summary>
        public IEnumerable<CustomEntityVersionRoute> Versions { get; set; }
    }
}
