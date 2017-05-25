using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SharedRouteLibrary : AngularModuleRouteLibrary
    {
        #region constructor

        public const string RoutePrefix = "shared";

        public SharedRouteLibrary(
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
            Html5ShivScriptPath = JsFile("html5shiv");
            MainCssPath = CssFile("shared");
        }

        #endregion

        #region Resources

        public string Html5ShivScriptPath { get; private set; }

        public string MainCssPath { get; private set; }

        #endregion
    }
}