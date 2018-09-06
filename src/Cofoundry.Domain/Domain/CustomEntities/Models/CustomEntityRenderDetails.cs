using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Data used for rendering a specific version of a custom entity
    /// out to a page, including template data for all the content-editable
    /// page regions. This object is specific to a particular version which 
    /// may not always be the latest (depending on the query), and to a specific
    /// page.
    /// </summary>
    public class CustomEntityRenderDetails : CustomEntityRenderSummary
    {
        /// <summary>
        /// Page region and fully mapped block data for a specific page 
        /// template, which will have been specified in the query used to 
        /// load this instance.
        /// </summary>
        public ICollection<CustomEntityPageRegionRenderDetails> Regions { get; set; }
    }
}
