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

        public DocumentsModuleRouteLibrary(
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