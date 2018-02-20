using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.Web;

namespace Cofoundry.Web.Admin
{
    public class AuthRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "auth";

        public readonly string LoginLayoutPath = ViewPathFormatter.View("Auth", "_LoginLayout");

        #endregion

        #region constructor

        public AuthRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string Login(string returnUrl = null)
        {
            var qs = QueryStringBuilder.Create("ReturnUrl", returnUrl);

            return MvcRoute("login", qs);
        }
        
        public string ChangePassword(string returnUrl = null)
        {
            var qs = QueryStringBuilder.Create("ReturnUrl", returnUrl);

            return MvcRoute("change-password", qs);
        }

        public string ForgotPassword()
        {
            return MvcRoute("forgot-password");
        }

        public string LogOut()
        {
            return MvcRoute("logout");
        }

        #endregion
    }
}