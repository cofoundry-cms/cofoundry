using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DocumentsModuleRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "documents";

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