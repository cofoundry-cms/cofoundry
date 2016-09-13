using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public static class SiteViewerModeExtensions
    {
        public static WorkFlowStatusQuery ToWorkFlowStatusQuery(this SiteViewerMode siteViewerMode)
        {
            switch (siteViewerMode)
            {
                case SiteViewerMode.Draft:
                case SiteViewerMode.Edit:
                    return WorkFlowStatusQuery.Draft;
                case SiteViewerMode.Any:
                    return WorkFlowStatusQuery.Latest;
                case SiteViewerMode.SpecificVersion:
                    return WorkFlowStatusQuery.SpecificVersion;
                default:
                    return WorkFlowStatusQuery.Published;
            }
        }
    }
}
