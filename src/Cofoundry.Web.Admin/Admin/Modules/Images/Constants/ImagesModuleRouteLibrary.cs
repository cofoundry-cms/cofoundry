using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class ImagesModuleRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "images";

        public static readonly ImagesModuleRouteLibrary Urls = new ImagesModuleRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public ImagesModuleRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string List()
        {
            return CreateAngularRoute();
        }

        public string New()
        {
            return CreateAngularRoute("new");
        }

        public string Details(int id)
        {
            return CreateAngularRoute(id.ToString());
        }

        #endregion
    }
}