using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Web.ModularMvc;
using Cofoundry.Domain;

namespace Cofoundry.Web.PageModules
{
    /// <summary>
    /// Bootstraps the registration of PageModule view locations, enabling PageModules to be included in non-standard places.
    /// </summary>
    public class PageModuleBootstrapper : IViewLocationRegistration
    {
        private readonly IPageModuleViewLocationRegistration[] _pageModuleViewLocationRegistrations;

        public PageModuleBootstrapper(
            IPageModuleViewLocationRegistration[] pageModuleViewLocationRegistrations
            )
        {
            _pageModuleViewLocationRegistrations = pageModuleViewLocationRegistrations;
        }
        
        public ViewLocations GetLocations()
        {
            // Register paths as partial views so that the standard view locator can find them.

            var pathsToRegister = _pageModuleViewLocationRegistrations
                .SelectMany(r => r.GetPathPrefixes())
                .Select(p => FormatViewPath(p))
                .Where(p => p != null)
                .ToArray();

            var locations = new ViewLocations()
            {
                PartialViewLocationFormats = pathsToRegister
            };

            return locations;
        }

        private string FormatViewPath(string pathPrefix)
        {
            if (string.IsNullOrWhiteSpace(pathPrefix)) return null;

            var path = "/" + pathPrefix.Trim('/') + "/{0}.cshtml";

            return path;
        }
    }
}
