using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SharedRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "shared";

        public static readonly SharedRouteLibrary Urls = new SharedRouteLibrary();

        public static readonly SharedJsRouteLibrary Js = new SharedJsRouteLibrary(Urls);

        public static readonly ModuleStaticContentRouteLibrary StaticContent = new ModuleStaticContentRouteLibrary(Urls);

        public static readonly SharedCssRouteLibrary Css = new SharedCssRouteLibrary(StaticContent);


        #endregion

        #region constructor

        public SharedRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}