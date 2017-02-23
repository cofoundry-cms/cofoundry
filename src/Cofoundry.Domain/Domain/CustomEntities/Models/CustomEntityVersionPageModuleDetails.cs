using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionPageModuleDetails
    {
        public int CustomEntityVersionPageModuleId { get; set; }

        public PageModuleTypeTemplateSummary Template { get; set; }

        public PageModuleTypeSummary ModuleType { get; set; }

        public IPageModuleDataModel DataModel { get; set; }
    }
}
