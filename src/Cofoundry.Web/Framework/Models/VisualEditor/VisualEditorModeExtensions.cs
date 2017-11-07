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
        public static PublishStatusQuery ToPublishStatusQuery(this VisualEditorMode siteViewerMode)
        {
            switch (siteViewerMode)
            {
                case VisualEditorMode.Draft:
                case VisualEditorMode.Edit:
                    return PublishStatusQuery.Draft;
                case VisualEditorMode.Any:
                    return PublishStatusQuery.Latest;
                case VisualEditorMode.SpecificVersion:
                    return PublishStatusQuery.SpecificVersion;
                default:
                    return PublishStatusQuery.Published;
            }
        }
    }
}
