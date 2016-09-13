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
    public class CustomEntityPageSectionRenderDetails : IEntitySectionRenderDetails<CustomEntityVersionPageModuleRenderDetails>
    {
        public int PageTemplateSectionId { get; set; }

        public string Name { get; set; }

        public CustomEntityVersionPageModuleRenderDetails[] Modules { get; set; }
    }
}
