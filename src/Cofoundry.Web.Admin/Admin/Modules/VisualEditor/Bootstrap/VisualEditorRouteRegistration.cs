using System;
using System.Collections.Generic;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorRouteRegistration : IOrderedRouteRegistration
    {
        private readonly PagesSettings _pagesSettings;

        public VisualEditorRouteRegistration(
            PagesSettings pagesSettings
            )
        {
            _pagesSettings = pagesSettings;
        }

        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            if (_pagesSettings.Disabled) return;

            routeBuilder
                .ForAdminController<VisualEditorController>(VisualEditorRouteLibrary.RoutePrefix)
                .MapRoute("Frame");
        }
    }
}
