using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class AccountRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "account";

        public AccountRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #region routes

        public string Details()
        {
            return AngularRoute();
        }

        #endregion
    }
}