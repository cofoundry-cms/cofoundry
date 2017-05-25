using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DirectoriesRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "directories";

        public static readonly DirectoriesRouteLibrary Urls = new DirectoriesRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public DirectoriesRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string List()
        {
            return AngularRoute();
        }

        public string New()
        {
            return AngularRoute("new");
        }

        public string Details(int id)
        {
            return AngularRoute(id.ToString());
        }

        #endregion
    }
}