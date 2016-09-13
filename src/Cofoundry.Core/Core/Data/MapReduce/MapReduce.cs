using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
{
    /// <summary>
    /// TPL MapReduce implementation
    /// </summary>
    /// <remarks>
    /// https://gist.github.com/prabirshrestha/2837187
    /// </remarks>
    public static class MapReduce
    {
        public static Task<TResult> Start<TInput, TPartialResult, TResult>(Func<TInput, TPartialResult> map, Func<TPartialResult[], TResult> reduce, params TInput[] inputs)
        {
            var mapTasks = CreateMapTasks(map, inputs);

            var reduceTask = CreateReduceTask(reduce, mapTasks);

            return reduceTask;
        }

        private static Task<TResult> CreateReduceTask<TPartialResult, TResult>(Func<TPartialResult[], TResult> reduce, Task<TPartialResult>[] mapTasks)
        {
            return Task.Factory.ContinueWhenAll(mapTasks, tasks => PerformReduce(reduce, tasks));
        }

        private static TResult PerformReduce<TPartialResult, TResult>(Func<TPartialResult[], TResult> reduce, Task<TPartialResult>[] tasks)
        {
            var results = tasks.Select(task => task.Result);
            return reduce(results.ToArray());
        }

        private static Task<TPartialResult>[] CreateMapTasks<TInput, TPartialResult>(Func<TInput, TPartialResult> map, TInput[] inputs)
        {
            var tasks = new Task<TPartialResult>[inputs.Length];

            for (int i = 0; i < inputs.Length; ++i)
            {
                var input = inputs[i];
                tasks[i] = Task.Factory.StartNew(() => map(input));
            }

            return tasks;
        }
    }
}
