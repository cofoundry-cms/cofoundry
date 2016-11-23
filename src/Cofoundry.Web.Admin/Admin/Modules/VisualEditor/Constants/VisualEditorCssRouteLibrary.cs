using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorCssRouteLibrary
    {
        #region constructor

        private readonly ModuleStaticContentRouteLibrary _moduleStaticContentRouteLibrary;

        public VisualEditorCssRouteLibrary(
            ModuleStaticContentRouteLibrary moduleStaticContentRouteLibrary
            )
        {
            _moduleStaticContentRouteLibrary = moduleStaticContentRouteLibrary;
        }

        #endregion

        /// <summary>
        /// Styles for the outer shell of the site viewer
        /// </summary>
<<<<<<< HEAD:src/Cofoundry.Web.Admin/Admin/Modules/SiteViewer/Constants/SiteViewerCssRouteLibrary.cs
        public string OuterSiteViewer
        {
            get
            {
                return Bundle("outersiteviewer");
            }
        }

        /// <summary>
        /// Style for site viewer content (i.e. page)
        /// </summary>
        public string InnerSiteViewer
        {
            get
            {
                return Bundle("innersiteviewer");
=======
        public string VisualEditor
        {
            get
            {
                return Bundle("visualeditor");
>>>>>>> refactored siteViewer refs to visualEditor:src/Cofoundry.Web.Admin/Admin/Modules/VisualEditor/Constants/VisualEditorCssRouteLibrary.cs
            }
        }

        public string Bundle(string path)
        {
            return "~" + _moduleStaticContentRouteLibrary.Url("css/" + path);
        }
    }
}