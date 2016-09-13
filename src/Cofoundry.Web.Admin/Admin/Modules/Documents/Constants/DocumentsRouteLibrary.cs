using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DocumentsRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "documents";

        public static readonly DocumentsRouteLibrary Urls = new DocumentsRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public DocumentsRouteLibrary()
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