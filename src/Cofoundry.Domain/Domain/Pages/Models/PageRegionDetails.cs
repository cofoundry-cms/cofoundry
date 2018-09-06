using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains block data for a single region in a specific
    /// page version. The block type used in this projection does
    /// not include the mapped display model, if you need the mapped
    /// data then use the PageRegionRenderDetails projection instead.
    /// </summary>
    public class PageRegionDetails
    {
        /// <summary>
        /// Database if of the template region.
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
        /// Ordered data for the all the blocks in this region.
        /// </summary>
        public ICollection<PageVersionBlockDetails> Blocks { get; set; }
    }
}
