using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IEntityVersionPageModuleRenderDetails
    {
        PageModuleTypeTemplateSummary Template { get; set; }

        PageModuleTypeSummary ModuleType { get; set; }

        IPageModuleDisplayModel DisplayModel { get; set; }

        int EntityVersionPageModuleId { get; set; }
    }
}
