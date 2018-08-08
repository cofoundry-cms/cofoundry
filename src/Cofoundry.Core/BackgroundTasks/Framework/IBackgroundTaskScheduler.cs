using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.BackgroundTasks
{
    /// <summary>
    /// Allows various types of background tasks to be scheduled.
    /// </summary>
    /// <remarks>
    /// This is only meant to be a simple implementation to support the needs of class libraries, in an actual
    /// application you'd most likely use a scheduler like HanFire directly to allow for more complex
    /// scenarios.
    /// </remarks>
    public interface IBackgroundTaskScheduler
    {
        /// <summary>
        /// Register a job to run every X number of days, at a specific time.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="days">The number of days to wait before executing the task again</param>
        /// <param name="atHour">The hour fo the day to execute the task (0-23)</param>
        /// <param name="atMinute">The minute of the hour to execute the task (0-60)</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int days, int atHour = 0, int atMinute = 0) where TTask : IRecurringBackgroundTask;

        /// <summary>
        /// Register a job to run every X number of hours.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="hours">The number of hours to wait before executing the task again</param>
        /// <param name="minute">The minute of the hour to execute the task (0-60)</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int hours = 0, int minute = 0) where TTask : IRecurringBackgroundTask;

        /// <summary>
        /// Register a job to run every X number of minutes.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="minutes">The number of minutes to wait before executing the task again</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterRecurringTask<TTask>(int minutes) where TTask : IRecurringBackgroundTask;

        /// <summary>
        /// Deregisters the specified job type (The class name/namespace is used as the identifier).
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler DeregisterRecurringTask<TTask>() where TTask : IRecurringBackgroundTask;

        /// <summary>
        /// Register a job to run every X number of days, at a specific time.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="days">The number of days to wait before executing the task again</param>
        /// <param name="atHour">The hour fo the day to execute the task (0-23)</param>
        /// <param name="atMinute">The minute of the hour to execute the task (0-60)</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int days, int atHour = 0, int atMinute = 0) where TTask : IAsyncRecurringBackgroundTask;

        /// <summary>
        /// Register a job to run every X number of hours.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="hours">The number of hours to wait before executing the task again</param>
        /// <param name="minute">The minute of the hour to execute the task (0-60)</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int hours = 0, int minute = 0) where TTask : IAsyncRecurringBackgroundTask;

        /// <summary>
        /// Register a job to run every X number of minutes.
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <param name="minutes">The number of minutes to wait before executing the task again</param>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler RegisterAsyncRecurringTask<TTask>(int minutes) where TTask : IAsyncRecurringBackgroundTask;

        /// <summary>
        /// Deregisters the specified job type (The class name/namespace is used as the identifier).
        /// </summary>
        /// <typeparam name="TTask">The type of task to execute</typeparam>
        /// <returns>Instance of IBackgroundTaskScheduler for method chaining</returns>
        IBackgroundTaskScheduler DeregisterAsyncRecurringTask<TTask>() where TTask : IAsyncRecurringBackgroundTask;
    }
}
