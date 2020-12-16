using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains block data for a single region in a custom entity page.
    /// This data is for a specific version of a custom entity. The block 
    /// type used in this projection does not include the mapped display 
    /// model, if you need the mapped data then use the 
    /// CustomEntityPageRegionRenderDetails projection instead.
    /// </summary>
    public class CustomEntityPageRegionDetails
    {
        /// <summary>
        /// Database id of the template region this instance 
        /// relates to.
        /// </summary>
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// The name of the template region. Region names can be any text string
        /// but will likely be alpha-numerical human readable names like 
        /// 'Heading' or 'Main Content'.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All block data for the region in this cusotm entity page. 
        /// This contains the dynamic content that gets rendered into
        /// the page template.
        /// </summary>
        public ICollection<CustomEntityVersionPageBlockDetails> Blocks { get; set; }
    }
}
