using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class AccountRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "account";

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
            return AngularRoute();
        }

        #endregion
    }
}