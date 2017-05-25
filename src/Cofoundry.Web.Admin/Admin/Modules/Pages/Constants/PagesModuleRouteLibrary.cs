using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "pages";

        #region constructor

        public PagesModuleRouteLibrary(
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

        public string WebDirectoryList()
        {
            return CreateWebDirectoryRoute();
        }

        #endregion

        #region helpers

        private string CreateWebDirectoryRoute(string path = null)
        {
            return "/" + UrlPrefix + "#/directories/" + path;
        }

        #endregion
    }
}