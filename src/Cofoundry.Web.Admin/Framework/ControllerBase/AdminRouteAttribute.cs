using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminRouteAttribute : RouteAttribute
    {
        protected AdminRouteAttribute()
            : base(RouteConstants.AdminAreaPrefix)
        {
        }

        public AdminRouteAttribute(string prefix)
            : base(RouteConstants.AdminAreaPrefix + "/" + prefix)
        {
        }
    }
}