using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class AdminApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var isAuthorized = base.IsAuthorized(actionContext);

            if (isAuthorized)
            {
                var service = GetService<IUserContextService>(actionContext);
                var user = service.GetCurrentContext();
                isAuthorized = user.IsCofoundryUser();
            }

            return isAuthorized;
        }

        private T GetService<T>(HttpActionContext actionContext)
        {
            var service = (T)actionContext.Request.GetDependencyScope().GetService(typeof(T)); ;
            return service;
        }
    }
}