using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SetupRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "setup";

        public static readonly SetupRouteLibrary Urls = new SetupRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        public const string SetupLayoutPath = "~/Admin/Modules/Setup/MVC/Views/_SetupLayout.cshtml";

        #endregion

        #region constructor

        public SetupRouteLibrary(
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

        public string Setup()
        {
            return MvcRoute();
        }

        #endregion
    }
}