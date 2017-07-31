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

        public string PageDirectoryList()
        {
            return CreatePageDirectoryRoute();
        }

        #endregion

        #region helpers

        private string CreatePageDirectoryRoute(string path = null)
        {
            return "/" + UrlPrefix + "#/directories/" + path;
        }

        #endregion
    }
}