using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents the input parameters of the Page action.
    /// </summary>
    public class PageActionInputParameters
    {
        /// <summary>
        /// The raw, relative path of the page without querystring
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Optionally a VersionId can be specified to
        /// view a specific version of a page or custom entity.
        /// </summary>
        public int? VersionId { get; set; }

        /// <summary>
        /// When routing to a custom entity this determines if we are editing the 
        /// custom entity or the overall page template. Both cannot be edited at the same
        /// time since it would be confusing to manage both version states.
        /// </summary>
        public bool IsEditingCustomEntity { get; set; }
    }
}
