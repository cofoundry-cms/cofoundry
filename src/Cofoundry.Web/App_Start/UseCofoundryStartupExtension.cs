using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public static class UseCofoundryStartupExtension
    {
        /// <summary>
        /// Registers Cofoundry into the Owin pipeline and runs all the registered
        /// Cofoundry StartupTasks. You must install and register a Cofoundry DI Integration 
        /// nuget package before calling this method (e.g. app.UseCofoundryAutoFacIntegration())
        /// </summary>
        /// <param name="application">Application configuration</param>
        /// <param name="configBuilder">Additional configuration options</param>
        public static void UseCofoundry(
            this IApplicationBuilder application,
            Action<CofoundryStartupConfiguration> configBuilder = null
            )
        {
            var configuration = new CofoundryStartupConfiguration();
            if (configBuilder != null) configBuilder(configuration);

            using (var childContext = application.ApplicationServices.CreateScope())
            {
                // Use the fullname secondry ordering here to get predicatable task ordering
                IEnumerable<IStartupTask> startupTasks = childContext
                    .ServiceProvider
                    .GetServices<IStartupTask>()
                    .OrderBy(i => i.Ordering)
                    .ThenBy(i => i.GetType().FullName);

                if (configuration.StartupTaskFilter != null)
                {
                    startupTasks = configuration.StartupTaskFilter(startupTasks);
                }

                foreach (var startupTask in startupTasks)
                {
                    startupTask.Run(application);
                }
            }
        }
    }
}