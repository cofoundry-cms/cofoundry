using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        #region overrides

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Not authenticated - go to login
                HandleUnauthenticatedRequest(filterContext);
            } 
            else 
            {
                HandleAuthenticatedRequest(filterContext);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (isAuthorized)
            {
                var user = GetUserContext();

                isAuthorized = user.IsCofoundryUser()
                    && (!user.IsPasswordChangeRequired || IsChangePasswordPage(httpContext.Request));
            }

            return isAuthorized;
        }

        #endregion

        #region helpers

        private IUserContext GetUserContext()
        {
            // No DI available here
            var userContextService = IckyDependencyResolution.ResolveFromMvcContext<IUserContextService>();
            return userContextService.GetCurrentContext();
        }

        private bool IsChangePasswordPage(HttpRequestBase request)
        {
            var passwordUrl = AuthRouteLibrary.Urls.ChangePassword();

            return request.RawUrl.StartsWith(passwordUrl);
        }

        private void HandleAuthenticatedRequest(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var user = GetUserContext();
            var passwordUrl = AuthRouteLibrary.Urls.ChangePassword();

            if (user.IsPasswordChangeRequired && !request.RawUrl.StartsWith(passwordUrl))
            {
                // Password change required
                passwordUrl = AuthRouteLibrary.Urls.ChangePassword(request.RawUrl);
                filterContext.Result = new RedirectResult(passwordUrl);
            }
            else
            {
                // Trying to access a forbidden resource
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                filterContext.Result = new ViewResult { ViewName = HttpStatusCode.Forbidden.ToString() };
            }
        }

        private static void HandleUnauthenticatedRequest(AuthorizationContext filterContext)
        {
            var loginUrl = AuthRouteLibrary.Urls.Login(filterContext.HttpContext.Request.RawUrl);
            filterContext.Result = new RedirectResult(loginUrl);
        }

        #endregion
    }
}