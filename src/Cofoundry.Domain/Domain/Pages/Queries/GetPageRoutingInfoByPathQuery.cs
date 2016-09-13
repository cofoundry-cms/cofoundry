using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Attempts to find a matching page route using the supplied path. The path
    /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
    /// </summary>
    public class GetPageRoutingInfoByPathQuery : IQuery<PageRoutingInfo>
    {
        /// <summary>
        /// Locale of the page or null if not using locales.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Path of the page to look for. Can include the locale segment, but it's ignored in 
        /// favour of the LocaleId in the query (to support cases where the locale may not be in the url)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Indicates whether to include unpublished page routes in the result.
        /// </summary>
        public bool IncludeUnpublished { get; set; }
    }
}
