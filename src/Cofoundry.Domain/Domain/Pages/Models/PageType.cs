using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Most pages are generic pages but they could have some sort of
    /// special function e.g. NotFound, CustomEntityDetails.
    /// </summary>
    public enum PageType
    {
        /// <summary>
        /// Standard page where you can define modular content
        /// </summary>
        Generic = 1,

        /// <summary>
        /// A page that displays the details of a custom entity. The custom entity data loaded in the
        /// page is dynamic based on a routing configuration.
        /// </summary>
        CustomEntityDetails = 2,

        /// <summary>
        /// A page to display if no matching page resources could be found in the containing directory.
        /// </summary>
        NotFound = 5,
    }
}
