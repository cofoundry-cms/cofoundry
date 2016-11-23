using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorJsRouteLibrary : ModuleJsRouteLibrary
    {
        public VisualEditorJsRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
            : base(moduleRouteLibrary)
        {
        }

        #region custom routes

<<<<<<< HEAD:src/Cofoundry.Web.Admin/Admin/Modules/SiteViewer/Constants/SiteViewerJsRouteLibrary.cs
        public string EventAggregator
        {
            get
            {
                return Bundle("EventAggregator");
=======
        public string VisualEditor
        {
            get
            {
                return Bundle("VisualEditor");
>>>>>>> refactored siteViewer refs to visualEditor:src/Cofoundry.Web.Admin/Admin/Modules/VisualEditor/Constants/VisualEditorJsRouteLibrary.cs
            }
        }

        #endregion
    }
}