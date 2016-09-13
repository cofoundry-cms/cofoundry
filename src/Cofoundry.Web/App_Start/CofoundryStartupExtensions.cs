using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Cofoundry.Web
{
    public static class CofoundryStartupExtensions
    {
        /// <summary>
        /// Registers Cofoundry into the Owin pipeline and runs all the registered
        /// Cofoundry StartupTasks. You must install and register a Cofoundry DI Integration 
        /// nuget package before calling this method (e.g. app.UseCofoundryAutoFacIntegration())
        /// </summary>
        /// <param name="app">Owin AppBuilder</param>
        /// <param name="configBuilder">Additional configuration options</param>
        public static void UseCofoundry(this IAppBuilder app, Action<CofoundryStartupConfiguration> configBuilder = null)
        {
            var configuration = new CofoundryStartupConfiguration();
            if (configBuilder != null) configBuilder(configuration);
            
            // We're not in a request scope here so open and close our own scope to tidy up resources
            using (var childContext = IckyDependencyResolution.CreateNewChildContextFromRoot())
            {
                // Use the fullname secondry ordering here to get predicatable task ordering
                IEnumerable<IStartupTask> startupTasks = childContext
                    .ResolveAll<IStartupTask>()
                    .OrderBy(i => i.Ordering)
                    .ThenBy(i => i.GetType().FullName);

                if (configuration.StartupTaskFilter != null)
                {
                    startupTasks = configuration.StartupTaskFilter(startupTasks);
                }

                foreach (var startupTask in startupTasks)
                {
                    startupTask.Run(app);
                }
            }
        }
    }
}