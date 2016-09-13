using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "custom-entities";

        public static readonly CustomEntitiesRouteLibrary Urls = new CustomEntitiesRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public CustomEntitiesRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}