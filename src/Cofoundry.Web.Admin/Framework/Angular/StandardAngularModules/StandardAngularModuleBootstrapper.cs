using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Registers instances of IStandardAngularModuleRegistration, which is used as a shortcut to 
    /// register angular admin modules that follow a standard pattern. RegisterRoutes needs to be manually called at the 
    /// point at which we want to routes added to the routing table, otherwise the rest should happen automatically.
    /// </summary>
    public class StandardAngularModuleRegistrationBootstrapper 
        : IEmbeddedResourceRouteRegistration
        , IAdminModuleRegistration
        , IOrderedRouteRegistration
    {
        #region constructor

        private readonly IEnumerable<IStandardAngularModuleRegistration> _standardAdminModuleRegistrations;
        private readonly AdminSettings _adminSettings;

        public StandardAngularModuleRegistrationBootstrapper(
            AdminSettings adminSettings,
            IEnumerable<IStandardAngularModuleRegistration> standardAdminModuleRegistrations
            )
        {
            _standardAdminModuleRegistrations = standardAdminModuleRegistrations;
            _adminSettings = adminSettings;
        }

        #endregion

        #region registration tasks

        public IEnumerable<AdminModule> GetModules()
        {
            return _standardAdminModuleRegistrations
                .Select(r => r.GetModule())
                .Where(m => m != null);
        }

        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var module = registration.GetModule();
                // if module null, then may be disabled
                if (module == null) continue;

                var routeLibrary = GetRouteLibrary(registration);

                routeBuilder
                    .ForAdminController<StandardAngularModuleController>(registration.RoutePrefix)
                    .MapIndexRoute(new { RouteLibrary = routeLibrary, ModuleTitle = module.Title });
            }
        }

        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
            if (_adminSettings.Disabled) yield break;

            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var routeLibrary = GetRouteLibrary(registration);
                var assembly = registration.GetType().Assembly;
                var path = new EmbeddedResourcePath(
                    assembly, 
                    routeLibrary.GetStaticResourceFilePath(), 
                    routeLibrary.GetStaticResourceUrlPath()
                    );

                yield return path;
            }
        }

        #endregion

        #region private helpers

        private AngularModuleRouteLibrary GetRouteLibrary(IStandardAngularModuleRegistration registration)
        {
            AngularModuleRouteLibrary routeLibrary;

            if (registration is IInternalAngularModuleRegistration)
            {
                // Internal modules are in a different folder format to prevent name clashes
                routeLibrary = new AngularModuleRouteLibrary(
                    _adminSettings,
                    registration.RoutePrefix, 
                    RouteConstants.InternalModuleResourcePathPrefix
                    );
            }
            else if (registration is IPluginAngularModuleRegistration)
            {
                // Plugin modules are in a different folder format to prevent name clashes
                routeLibrary = new AngularModuleRouteLibrary(
                    _adminSettings,
                    registration.RoutePrefix, 
                    RouteConstants.PluginModuleResourcePathPrefix
                    );
            }
            else
            {
                routeLibrary = new AngularModuleRouteLibrary(_adminSettings, registration.RoutePrefix);
            }

            return routeLibrary;
        }
        
        #endregion
    }
}