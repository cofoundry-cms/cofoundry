using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class UsersRouteLibrary : ModuleRouteLibrary
    {
        public const string RoutePrefix = "users";
        
        public UsersRouteLibrary(
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
    }
}