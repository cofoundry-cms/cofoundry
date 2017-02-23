using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A lighter weight version of PageSectionRender details
    /// without full display model mapping for modules. Includes 
    /// only raw data model data.
    /// </summary>
    public class PageSectionDetails
    {
        public int PageTemplateSectionId { get; set; }

        public string Name { get; set; }

        public PageVersionModuleDetails[] Modules { get; set; }
    }
}
