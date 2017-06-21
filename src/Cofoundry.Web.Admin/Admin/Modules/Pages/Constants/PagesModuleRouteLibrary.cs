using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class PagesModuleRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "pages";

        #region constructor

        public PagesModuleRouteLibrary()
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