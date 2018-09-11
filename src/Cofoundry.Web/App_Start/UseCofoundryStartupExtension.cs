using Cofoundry.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public static class UseCofoundryStartupExtension
    {
        /// <summary>
        /// Registers Cofoundry into the application pipeline and runs all the registered
        /// Cofoundry StartupTasks.
        /// </summary>
        /// <param name="application">Application configuration.</param>
        /// <param name="configBuilder">Additional configuration options.</param>
        public static void UseCofoundry(
            this IApplicationBuilder application,
            Action<UseCofoundryStartupConfiguration> configBuilder = null
            )
        {
            var configuration = new UseCofoundryStartupConfiguration();
            if (configBuilder != null) configBuilder(configuration);

            using (var childContext = application.ApplicationServices.CreateScope())
            {
                var startupTasks = childContext
                    .ServiceProvider
                    .GetServices<IStartupConfigurationTask>();

                startupTasks = SortTasksByDependency(startupTasks.ToArray());

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

        private class StartupTaskLookupItem
        {
            public StartupTaskLookupItem(IStartupConfigurationTask startupTask)
            {
                StartupTask = startupTask;
            }
            public IStartupConfigurationTask StartupTask { get; set; }

            public List<IStartupConfigurationTask> Dependencies { get; set; } = new List<IStartupConfigurationTask>();
        }

        private static ICollection<IStartupConfigurationTask> SortTasksByDependency(ICollection<IStartupConfigurationTask> startupTasks)
        {
            // Set up a lookup of task
            var startupTaskLookup = startupTasks.ToDictionary(k => k.GetType(), v => new StartupTaskLookupItem(v));

            foreach (var startupTask in startupTasks
                .Where(t => t is IRunAfterStartupConfigurationTask)
                .Cast<IRunAfterStartupConfigurationTask>())
            {
                var startupTaskLookupItem = startupTaskLookup[startupTask.GetType()];

                foreach (var runAfterTaskType in startupTask.RunAfter)
                {
                    var dependentTask = startupTaskLookup[runAfterTaskType].StartupTask;
                    startupTaskLookupItem.Dependencies.Add(dependentTask);
                }
            }

            foreach (var startupTask in startupTasks
                .Where(t => t is IRunBeforeStartupConfigurationTask)
                .Cast<IRunBeforeStartupConfigurationTask>())
            {
                foreach (var runBeforeTaskType in startupTask.RunBefore)
                {
                    startupTaskLookup[runBeforeTaskType].Dependencies.Add(startupTask);
                }
            }

            // Pre-sort by numerical task ordering
            // The fullname secondry ordering is used to get predicatable task ordering
            var orderedTasks = startupTasks
                .OrderBy(t => t.Ordering)
                .ThenBy(t => t.GetType().FullName)
                .ToArray();

            try
            {
                // Then do a Topological Sort based on task dependencies
                return TopologicalSorter.Sort(orderedTasks, (task, source) => startupTaskLookup[task.GetType()].Dependencies, true);
            }
            catch (CyclicDependencyException ex)
            {
                throw new CyclicDependencyException("A cyclic dependency has been detected between multiple IStartupConfigurationTask classes. Check your startup tasks to ensure they do not depend on each other. For more details see the inner exception message.", ex);
            }
        }
    }
}