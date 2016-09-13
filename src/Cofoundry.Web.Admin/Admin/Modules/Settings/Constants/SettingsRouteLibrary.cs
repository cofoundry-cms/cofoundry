using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SettingsRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "settings";

        public static readonly SettingsRouteLibrary Urls = new SettingsRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public SettingsRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {

        }

        #endregion

        #region routes

        public string Details()
        {
            return CreateAngularRoute();
        }

        #endregion
    }
}