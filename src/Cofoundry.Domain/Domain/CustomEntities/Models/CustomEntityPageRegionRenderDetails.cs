using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page region and fully mapped block data for a custom entity. This is used 
    /// for rendering out to a custom entitity details page.
    /// </summary>
    public class CustomEntityPageRegionRenderDetails : IEntityRegionRenderDetails<CustomEntityVersionPageBlockRenderDetails>
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
        /// Collection of fully mapped blocks including display models.
        /// </summary>
        public ICollection<CustomEntityVersionPageBlockRenderDetails> Blocks { get; set; }
    }
}
