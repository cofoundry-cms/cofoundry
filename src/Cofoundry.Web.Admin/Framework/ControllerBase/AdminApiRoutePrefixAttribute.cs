using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace Cofoundry.Web.Admin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminApiRoutePrefixAttribute : Attribute, IRoutePrefix
    {
        protected AdminApiRoutePrefixAttribute()
        {
        }

        public AdminApiRoutePrefixAttribute(string prefix)
        {
            Prefix = RouteConstants.ApiRoutePrefix + "/" + prefix;
        }

        public string Prefix { get; private set; }
    }
}