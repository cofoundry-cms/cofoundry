using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains all the information required to render a page module out to a page.
    /// </summary>
    public class PageVersionModuleRenderDetails : IEntityVersionPageModuleRenderDetails
    {
        public int PageVersionModuleId { get; set; }

        /// <summary>
        /// A module can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out.
        /// If no template is set then the default view is used for rendering.
        /// </summary>
        public PageModuleTypeTemplateSummary Template { get; set; }

        public PageModuleTypeSummary ModuleType { get; set; }

        public IPageModuleDisplayModel DisplayModel { get; set; }

        public int EntityVersionPageModuleId
        {
            get { return PageVersionModuleId; }
            set { PageVersionModuleId = value; }
        }
    }
}
