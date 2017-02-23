using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageVersionModuleDetails
    {
        public int PageVersionModuleId { get; set; }

        /// <summary>
        /// A module can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out.
        /// If no template is set then the default view is used for rendering.
        /// </summary>
        public PageModuleTypeTemplateSummary Template { get; set; }

        public PageModuleTypeSummary ModuleType { get; set; }

        public IPageModuleDataModel DataModel { get; set; }
    }
}
