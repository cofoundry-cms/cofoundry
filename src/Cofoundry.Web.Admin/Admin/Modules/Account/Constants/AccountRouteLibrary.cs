using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class AccountRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "account";

        public static readonly AccountRouteLibrary Urls = new AccountRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public AccountRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string Details()
        {
            return CreateAngularRoute();
        }

        #endregion
    }
}