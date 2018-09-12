using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    public class AuthRouteRegistration : IOrderedRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Cofoundry Admin - Root default action",
                RouteConstants.AdminAreaPrefix,
                new { controller = "Auth", action = "DefaultRedirect", Area = RouteConstants.AdminAreaName }
                );

            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Auth:ForgotPassword",
                RouteConstants.AdminAreaPrefix + "/auth/forgot-password",
                new { controller = "Auth", action = "ForgotPassword", Area = RouteConstants.AdminAreaName }
                );

            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Auth:ResetPassword",
                RouteConstants.AdminAreaPrefix + "/auth/reset-password",
                new { controller = "Auth", action = "ResetPassword", Area = RouteConstants.AdminAreaName }
                );

            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Auth:ChangePassword",
                RouteConstants.AdminAreaPrefix + "/auth/change-password",
                new { controller = "Auth", action = "ChangePassword", Area = RouteConstants.AdminAreaName }
                );

            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Auth",
                RouteConstants.AdminAreaPrefix + "/auth/{action}",
                new { controller = "Auth", action = "Index", Area = RouteConstants.AdminAreaName }
                );
        }
    }
}
