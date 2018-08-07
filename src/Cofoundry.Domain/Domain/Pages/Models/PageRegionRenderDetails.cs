using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page region information for rendering a page including 
    /// full block details with mapped display models.
    /// </summary>
    public class PageRegionRenderDetails : IEntityRegionRenderDetails<PageVersionBlockRenderDetails>
    {
        /// <summary>
        /// Database id of the page template region record.
        /// </summary>
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// The name identifier for a region. Region names can be any text string 
        /// but will likely be alpha-numerical human readable names like 'Heading', 
        /// 'Main Content'. Region names are unique (non-case sensitive) for the 
        /// template they belong to.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of fully mapped and ordered blocks including display models.
        /// </summary>
        public ICollection<PageVersionBlockRenderDetails> Blocks { get; set; }
    }
}
