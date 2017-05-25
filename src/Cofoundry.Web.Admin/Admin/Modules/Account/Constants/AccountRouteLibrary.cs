using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class AccountRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "account";

        #endregion

        #region constructor

        public AccountRouteLibrary(
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

        public string Details()
        {
            return AngularRoute();
        }

        #endregion
    }
}