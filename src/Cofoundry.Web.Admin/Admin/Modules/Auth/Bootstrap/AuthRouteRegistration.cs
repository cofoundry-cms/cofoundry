using System;
using System.Collections.Generic;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    public class AuthRouteRegistration : IOrderedRouteRegistration
    {
        private readonly AdminSettings _adminSettings;

        public AuthRouteRegistration(
            AdminSettings adminSettings
            )
        {
            _adminSettings = adminSettings;
        }

        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            // this is usually handled by ForAdminController etc but
            // since we have a custom route for the default redirect we need
            // to check it here.
            if (_adminSettings.Disabled) return;

            routeBuilder.ForAdminController<AuthController>("auth")
                .MapIndexRoute()
                .MapRoute("Login")
                .MapRoute("Logout")
                .MapRoute("ForgotPassword")
                .MapRoute("ResetPassword")
                .MapRoute("ChangePassword");

            routeBuilder.MapControllerRoute(
                "Cofoundry Admin - Root default action",
                _adminSettings.DirectoryName,
                new { controller = "Auth", action = "DefaultRedirect", Area = RouteConstants.AdminAreaName }
                );

        }
    }
}
