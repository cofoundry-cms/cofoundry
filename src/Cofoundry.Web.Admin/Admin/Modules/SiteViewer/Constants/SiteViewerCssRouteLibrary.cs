using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerCssRouteLibrary
    {
        #region constructor

        private readonly ModuleStaticContentRouteLibrary _moduleStaticContentRouteLibrary;

        public SiteViewerCssRouteLibrary(
            ModuleStaticContentRouteLibrary moduleStaticContentRouteLibrary
            )
        {
            _moduleStaticContentRouteLibrary = moduleStaticContentRouteLibrary;
        }

        #endregion

        /// <summary>
        /// Styles for the outer shell of the site viewer
        /// </summary>
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
            }
        }

        public string Bundle(string path)
        {
            return "~" + _moduleStaticContentRouteLibrary.Url("css/" + path);
        }
    }
}