using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public static class VisualEditorModeExtensions
    {
        public static WorkFlowStatusQuery ToWorkFlowStatusQuery(this VisualEditorMode siteViewerMode)
        {
            switch (siteViewerMode)
            {
                case VisualEditorMode.Draft:
                case VisualEditorMode.Edit:
                    return WorkFlowStatusQuery.Draft;
                case VisualEditorMode.Any:
                    return WorkFlowStatusQuery.Latest;
                case VisualEditorMode.SpecificVersion:
                    return WorkFlowStatusQuery.SpecificVersion;
                default:
                    return WorkFlowStatusQuery.Published;
            }
        }
    }
}
