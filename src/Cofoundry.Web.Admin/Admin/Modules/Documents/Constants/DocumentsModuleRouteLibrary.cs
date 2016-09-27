using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DocumentsModuleRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "documents";

        public static readonly DocumentsModuleRouteLibrary Urls = new DocumentsModuleRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public DocumentsModuleRouteLibrary()
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