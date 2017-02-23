using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityPageSectionDetails
    {
        public int PageTemplateSectionId { get; set; }

        public string Name { get; set; }

        public CustomEntityVersionPageModuleDetails[] Modules { get; set; }
    }
}
