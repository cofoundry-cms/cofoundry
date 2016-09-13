using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionPageModuleRenderDetails : IEntityVersionPageModuleRenderDetails
    {
        public int CustomEntityVersionPageModuleId { get; set; }

        public PageModuleTypeTemplateSummary Template { get; set; }

        public PageModuleTypeSummary ModuleType { get; set; }

        public IPageModuleDisplayModel DisplayModel { get; set; }

        public int EntityVersionPageModuleId
        {
            get { return CustomEntityVersionPageModuleId; }
            set { CustomEntityVersionPageModuleId = value; }
        }
    }
}
