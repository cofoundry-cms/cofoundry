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
        public string SiteViewer
        {
            get
            {
                return Bundle("siteviewer");
            }
        }

        public string Bundle(string path)
        {
            return "~" + _moduleStaticContentRouteLibrary.Url("css/" + path);
        }
    }
}