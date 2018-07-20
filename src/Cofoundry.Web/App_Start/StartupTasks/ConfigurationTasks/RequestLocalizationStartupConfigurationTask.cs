using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cofoundry.Web.App_Start.StartupTasks.ConfigurationTasks
{
    /// <summary>
    /// Represents a task that runs in the Cofoundry startup and 
    /// initialization pipeline.
    /// </summary>
    public class RequestLocalizationStartupConfigurationTask : IStartupConfigurationTask
    {
        /// <summary>
        /// An integer representing the ordering (lower values first). Can use custom 
        /// values but using the enum StartupTaskOrdering is recommended.
        /// </summary>
        public int Ordering => (int) StartupTaskOrdering.Early;

        /// <summary>
        /// Executes the configuration task
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            if (locOptions.Value != null)
            {
                app.UseRequestLocalization(locOptions.Value);
            }
        }
    }
}
