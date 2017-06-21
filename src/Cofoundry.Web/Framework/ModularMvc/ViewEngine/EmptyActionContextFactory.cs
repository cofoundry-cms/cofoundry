using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public class EmptyActionContextFactory : IEmptyActionContextFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<ActionContext> _actionContext;

        public EmptyActionContextFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
            _actionContext = new Lazy<ActionContext>(CreateActionContext);
        }

        public ActionContext Create()
        {
            return _actionContext.Value;
        }

        private ActionContext CreateActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}