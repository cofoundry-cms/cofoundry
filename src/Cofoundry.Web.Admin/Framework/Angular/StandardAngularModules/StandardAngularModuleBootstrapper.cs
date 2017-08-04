using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Registers instances of IStandardAngularModuleRegistration, which is used as a shortcut to 
    /// register angular admin modules that follow a standard pattern. RegisterRoutes needs to be manually called at the 
    /// point at which we want to routes added to the routing table, otherwise the rest should happen automatically.
    /// </summary>
    public class StandardAngularModuleRegistrationBootstrapper : IEmbeddedResourceRouteRegistration, IAdminModuleRegistration, IRouteRegistration
    {
        #region constructor

        private readonly IEnumerable<IStandardAngularModuleRegistration> _standardAdminModuleRegistrations;

        public StandardAngularModuleRegistrationBootstrapper(
            IEnumerable<IStandardAngularModuleRegistration> standardAdminModuleRegistrations
            )
        {
            _standardAdminModuleRegistrations = standardAdminModuleRegistrations;
        }

        #endregion

        #region registration tasks

        public IEnumerable<AdminModule> GetModules()
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                yield return registration.GetModule();
            }
        }

        public void RegisterRoutes(IRouteBuilder routes)
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var routeLibrary = GetRouteLibrary(registration);

                routes.MapRoute(
                    "Cofoundry Admin Module - " + registration.RoutePrefix,
                    RouteConstants.AdminAreaPrefix + "/" + registration.RoutePrefix,
                    new { controller = "StandardModule", action = "Index", Area = RouteConstants.AdminAreaName },
                    null,
                    new { RouteLibrary = routeLibrary }
                    );
            }
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var routeLibrary = GetRouteLibrary(registration);

                yield return routeLibrary.StaticResourcePrefix;
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
                    registration.RoutePrefix, 
                    RouteConstants.InternalModuleResourcePathPrefix
                    );
            }
            else if (registration is IPluginAngularModuleRegistration)
            {
                // Internal modules are in a different folder format to prevent name clashes
                routeLibrary = new AngularModuleRouteLibrary(
                    registration.RoutePrefix, 
                    RouteConstants.PluginModuleResourcePathPrefix
                    );
            }
            else
            {
                routeLibrary = new AngularModuleRouteLibrary(registration.RoutePrefix);
            }

            return routeLibrary;
        }
        
        #endregion
    }
}