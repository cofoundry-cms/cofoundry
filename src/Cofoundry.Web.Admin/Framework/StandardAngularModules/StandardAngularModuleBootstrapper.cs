using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Cofoundry.Web.ModularMvc;
using Cofoundry.Domain;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Registers instances of IStandardAngularModuleRegistration, which is used as a shortcut to 
    /// register angular admin modules that follow a standard pattern. RegisterRoutes needs to be manually called at the 
    /// point at which we want to routes added to the routing table, otherwise the rest should happen automatically.
    /// </summary>
    public class StandardAngularModuleRegistrationBootstrapper : IEmbeddedResourceRouteRegistration, IBundleRegistration, IAdminModuleRegistration, IRouteRegistration
    {
        #region constructor

        private readonly IStandardAngularModuleRegistration[] _standardAdminModuleRegistrations;

        public StandardAngularModuleRegistrationBootstrapper(
            IStandardAngularModuleRegistration[] standardAdminModuleRegistrations
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

        public void RegisterRoutes(RouteCollection routes)
        {
            var controllerNamespace = new string[] { typeof(StandardModuleController).Namespace };

            foreach (var registration in _standardAdminModuleRegistrations)
            {
                ModuleRouteLibrary routeLibrary = GetRouteLibrary(registration);

                var jsRouteLibrary = new ModuleJsRouteLibrary(routeLibrary);

                routes.MapRoute(
                    "Cofoundry Admin Module - " + registration.RoutePrefix,
                    RouteConstants.AdminAreaPrefix + "/" + registration.RoutePrefix,
                    new { controller = "StandardModule", action = "Index", routeLibrary = routeLibrary, Area = RouteConstants.AdminAreaName },
                    controllerNamespace
                    );
            }
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var jsRouteLibrary = GetJsRouteLibrary(registration);
                yield return jsRouteLibrary.JsFolderPath;
            }
        }

        public void RegisterBundles(System.Web.Optimization.BundleCollection bundles)
        {
            foreach (var registration in _standardAdminModuleRegistrations)
            {
                var jsRouteLibrary = GetJsRouteLibrary(registration);

                bundles.AddMainAngularScriptBundle(jsRouteLibrary,
                    AngularJsDirectoryLibrary.Bootstrap,
                    AngularJsDirectoryLibrary.Routes,
                    AngularJsDirectoryLibrary.Filters,
                    AngularJsDirectoryLibrary.DataServices,
                    AngularJsDirectoryLibrary.UIComponents);

                bundles.AddAngularTemplateBundle(jsRouteLibrary,
                    AngularJsDirectoryLibrary.Routes,
                    AngularJsDirectoryLibrary.UIComponents);
            }
        }

        #endregion

        #region private helpers

        private ModuleRouteLibrary GetRouteLibrary(IStandardAngularModuleRegistration registration)
        {
            ModuleRouteLibrary routeLibrary;

            if (registration is IInternalAngularModuleRegistration)
            {
                // Internal modules are in a different folder format to prevent name clashes
                routeLibrary = new ModuleRouteLibrary(registration.RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix);
            }
            else if (registration is IPluginAngularModuleRegistration)
            {
                // Internal modules are in a different folder format to prevent name clashes
                routeLibrary = new ModuleRouteLibrary(registration.RoutePrefix, RouteConstants.PluginModuleResourcePathPrefix);
            }
            else
            {
                routeLibrary = new ModuleRouteLibrary(registration.RoutePrefix);
            }

            return routeLibrary;
        }

        private ModuleJsRouteLibrary GetJsRouteLibrary(IStandardAngularModuleRegistration registration)
        {
            var routeLibrary = GetRouteLibrary(registration);
            var jsRouteLibrary = new ModuleJsRouteLibrary(routeLibrary);

            return jsRouteLibrary;
        }

        #endregion
    }
}