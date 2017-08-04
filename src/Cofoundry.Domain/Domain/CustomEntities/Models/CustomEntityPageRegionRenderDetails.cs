using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// For displaying a page
    /// </summary>
    public class CustomEntityPageRegionRenderDetails : IEntityRegionRenderDetails<CustomEntityVersionPageBlockRenderDetails>
    {
        public int PageTemplateRegionId { get; set; }

        public string Name { get; set; }

        public CustomEntityVersionPageBlockRenderDetails[] Blocks { get; set; }
    }
}
