using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "site-viewer";

        public static readonly SiteViewerRouteLibrary Urls = new SiteViewerRouteLibrary();

        public static readonly ModuleStaticContentRouteLibrary StaticContent = new ModuleStaticContentRouteLibrary(Urls);

        public static readonly SiteViewerCssRouteLibrary Css = new SiteViewerCssRouteLibrary(StaticContent);

        public static readonly SiteViewerJsRouteLibrary Js = new SiteViewerJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public SiteViewerRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {

        }

        #endregion

        #region resources

        public static string SiteViewerViewPath()
        {
            return Urls.ResourcePrefix + "mvc/views/SiteViewer.cshtml";
        }

        public static string SiteViewerToolbarViewPath()
        {
            return Urls.ResourcePrefix + "mvc/views/Toolbar.cshtml";
        }

        #endregion
    }
}