using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageModuleTypeSummary
    {
        public int PageModuleTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// Indictaes whther this is a custom module type implemented outside of the core
        /// Cofoundry system.
        /// </summary>
        public bool IsCustom { get; set; }

        public IEnumerable<PageModuleTypeTemplateSummary> Templates { get; set; }
    }
}
