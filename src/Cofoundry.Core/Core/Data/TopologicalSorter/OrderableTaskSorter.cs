using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Sorts tasks taking into account ordering via IOrderableTask and
    /// dependent tasks via IRunAfterTask or IRunBeforeTask implementations.
    /// </summary>
    public static class OrderableTaskSorter
    {
        private class TaskLookupItem<TTask>
        {
            public TaskLookupItem(TTask task)
            {
                Task = task;
            }
            public TTask Task { get; set; }

            public List<TTask> Dependencies { get; set; } = new List<TTask>();
        }

        /// <summary>
        /// Sorts tasks taking into account ordering via IOrderableTask and
        /// dependent tasks via IRunAfterTask or IRunBeforeTask implementations.
        /// </summary>
        /// <param name="tasks">
        /// The collection of tasks to sort. For any ties or items without 
        /// dependencies the type name is used to provide a determinaistic 
        /// secondary ordering.
        /// </param>
        public static ICollection<TTask> Sort<TTask>(IEnumerable<TTask> tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));

            var orderedTask = tasks
                .OrderBy(r => GetOrdering(r))
                .ThenBy(r => r.GetType().Name)
                .ToArray();

            // Set up a lookup of task
            var taskLookup = orderedTask.ToDictionary(k => k.GetType(), v => new TaskLookupItem<TTask>(v));

            foreach (var task in orderedTask
                .Where(t => t is IRunAfterTask)
                .Cast<IRunAfterTask>()
                .Where(t => !EnumerableHelper.IsNullOrEmpty(t.RunAfter)))
            {
                var taskLookupItem = taskLookup[task.GetType()];

                foreach (var runAfterTaskType in task.RunAfter)
                {
                    var dependentTask = taskLookup[runAfterTaskType].Task;
                    taskLookupItem.Dependencies.Add(dependentTask);
                }
            }

            foreach (var task in orderedTask
                .Where(t => t is IRunBeforeTask)
                .Cast<IRunBeforeTask>()
                .Where(t => !EnumerableHelper.IsNullOrEmpty(t.RunBefore)))
            {
                foreach (var runBeforeTaskType in task.RunBefore)
                {
                    taskLookup[runBeforeTaskType].Dependencies.Add((TTask)task);
                }
            }

            // Then do a Topological Sort based on task dependencies
            return TopologicalSorter.Sort(orderedTask, (task, source) => taskLookup[task.GetType()].Dependencies, true);
        }

        private static int GetOrdering<TTask>(TTask task)
        {
            if (task is IOrderedTask orderedtask)
            {
                return orderedtask.Ordering;
            }

            return 0;
        }
    }
}
