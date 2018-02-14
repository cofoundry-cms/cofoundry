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
        public int PageTemplateRegionId { get; set; }

        public string Name { get; set; }

        public ICollection<PageVersionBlockRenderDetails> Blocks { get; set; }
    }
}
