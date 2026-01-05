using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// Extension methods for configuring Cofoundry middleware via <see cref="IApplicationBuilder"/>.
/// </summary>
public static class UseCofoundryStartupExtension
{
    extension(IApplicationBuilder application)
    {
        /// <summary>
        /// Registers Cofoundry into the application pipeline and runs all the registered
        /// Cofoundry StartupTasks.
        /// </summary>
        /// <param name="configBuilder">Additional configuration options.</param>
        public void UseCofoundry(
            Action<UseCofoundryStartupConfiguration>? configBuilder = null
            )
        {
            var configuration = new UseCofoundryStartupConfiguration();
            configBuilder?.Invoke(configuration);

            using var childContext = application.ApplicationServices.CreateScope();

            var startupTasks = childContext
                .ServiceProvider
                .GetServices<IStartupConfigurationTask>();

            startupTasks = SortTasksByDependency(startupTasks);

            if (configuration.StartupTaskFilter != null)
            {
                startupTasks = configuration.StartupTaskFilter(startupTasks);
            }

            foreach (var startupTask in startupTasks)
            {
                startupTask.Configure(application);
            }
        }
    }

    private static IReadOnlyCollection<IStartupConfigurationTask> SortTasksByDependency(IEnumerable<IStartupConfigurationTask> startupTasks)
    {
        try
        {
            // Do a Topological Sort based on task dependencies
            return OrderableTaskSorter.Sort(startupTasks);
        }
        catch (CyclicDependencyException ex)
        {
            throw new CyclicDependencyException("A cyclic dependency has been detected between multiple IStartupConfigurationTask classes. Check your startup tasks to ensure they do not depend on each other. For more details see the inner exception message.", ex);
        }
    }
}
