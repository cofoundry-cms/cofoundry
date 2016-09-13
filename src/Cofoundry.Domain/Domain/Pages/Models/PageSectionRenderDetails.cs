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
    public class PageSectionRenderDetails : IEntitySectionRenderDetails<PageVersionModuleRenderDetails>
    {
        public int PageTemplateSectionId { get; set; }

        public string Name { get; set; }

        public PageVersionModuleRenderDetails[] Modules { get; set; }
    }
}
