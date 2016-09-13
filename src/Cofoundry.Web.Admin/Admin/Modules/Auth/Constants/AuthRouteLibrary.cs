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

        public static readonly AuthRouteLibrary Urls = new AuthRouteLibrary();

        public static readonly AuthJSRouteLibrary Js = new AuthJSRouteLibrary(Urls);

        public static readonly ModuleStaticContentRouteLibrary StaticContent = new ModuleStaticContentRouteLibrary(Urls);

        public static readonly AuthCssRouteLibrary Css = new AuthCssRouteLibrary(StaticContent);

        public const string LoginLayoutPath = "~/Admin/Modules/Auth/MVC/Views/_LoginLayout.cshtml";

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

            return CreateMvcRoute("login", qs);
        }

        public string LoginWithEmail(string email)
        {
            var qs = QueryStringBuilder.Create("email", email);

            return CreateMvcRoute("login", qs);
        }

        public string ChangePassword(string returnUrl = null)
        {
            var qs = QueryStringBuilder.Create("ReturnUrl", returnUrl);

            return CreateMvcRoute("change-password", qs);
        }

        public string ForgotPassword(string email = null)
        {
            var qs = QueryStringBuilder.Create("email", email);
            return CreateMvcRoute("forgot-password", qs);
        }

        public string LogOut()
        {
            return CreateMvcRoute("logout");
        }

        #endregion
    }
}