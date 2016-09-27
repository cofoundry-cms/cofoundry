using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "pages";

        public static readonly PagesModuleRouteLibrary Urls = new PagesModuleRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);


        #endregion

        #region constructor

        public PagesModuleRouteLibrary()
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