using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class ImagesModuleRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "images";

        public static readonly ImagesModuleRouteLibrary Urls = new ImagesModuleRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor
        
        public ImagesModuleRouteLibrary(
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