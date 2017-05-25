using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "visual-editor";

        public static readonly VisualEditorRouteLibrary Urls = new VisualEditorRouteLibrary();

        public static readonly ModuleStaticContentRouteLibrary StaticContent = new ModuleStaticContentRouteLibrary(Urls);

        public static readonly VisualEditorCssRouteLibrary Css = new VisualEditorCssRouteLibrary(StaticContent);

        public static readonly VisualEditorJsRouteLibrary Js = new VisualEditorJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public VisualEditorRouteLibrary(
            IStaticResourceFileProvider staticResourceFileProvider,
            OptimizationSettings optimizationSettings
            )
            : base(
                  RoutePrefix,
                  RouteConstants.InternalModuleResourcePathPrefix,
                  staticResourceFileProvider,
                  optimizationSettings
                  )
        {
        }

        #endregion

        #region resources

        public static string VisualEditorToolbarViewPath()
        {
            return Urls.ResourcePrefix + "mvc/views/Toolbar.cshtml";
        }

        #endregion
    }
}