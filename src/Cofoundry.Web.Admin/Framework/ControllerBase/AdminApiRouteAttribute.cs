using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminApiRouteAttribute : RouteAttribute
    {
        protected AdminApiRouteAttribute()
            : base(RouteConstants.ApiRoutePrefix)
        {
        }

        public AdminApiRouteAttribute(string prefix)
            : base(RouteConstants.ApiRoutePrefix + "/" + prefix)
        {
        }
    }
}