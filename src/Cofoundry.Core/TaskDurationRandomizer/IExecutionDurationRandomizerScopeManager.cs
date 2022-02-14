namespace Cofoundry.Core.ExecutionDurationRandomizer
{
    /// <summary>
    /// <para>
    /// The execution duration randomizer feature prevents execution completing before a random duration 
    /// has elapsed by padding the execution time using <see cref="Task.Delay"/>. This can help mitigate 
    /// against time-based enumeration attacks by extending the <paramref name="minDurationInMilliseconds"/> beyond 
    /// the expected bounds of the completion time. For example, this could be used to mitigate harvesting 
    /// of valid usernames from login or forgot password pages by measuring the response times.
    /// </para>
    /// <para>
    /// This manager class is main API for the feature and and is used to control the scope of one or more 
    /// <see cref="IExecutionDurationRandomizerScope"/> instances during the lifetime of a request. This ensures 
    /// that the top-level (primary) scope controls the duration of the overall task execution, while inner or child 
    /// scopes cannot close the scope early. Any child scopes may extend the duration if they are configured
    /// with a longer duration parameters.
    /// </para>
    /// </summary>
    public interface IExecutionDurationRandomizerScopeManager
    {
        /// <summary>
        /// Creates a new scope to manage the duration of an executing task. This is
        /// typically invoked with an async using statement, whereby the
        /// duration extension is handled when the scope is disposed.
        /// </summary>
        /// <param name="duration">
        /// The duration requirements to assign to the scope.
        /// </param>
        IExecutionDurationRandomizerScope Create(RandomizedExecutionDuration duration);

        /// <summary>
        /// Creates a new scope to manage the duration of an execution task. This is
        /// typically invoked with an async using statement, whereby the
        /// duration extension is handled when the scope is disposed.
        /// </summary>
        /// <param name="minDurationInMilliseconds">
        /// The inclusive lower bound of the randomized duration, measured in 
        /// milliseconds (1000ms = 1s).
        /// </param>
        /// <param name="maxDurationInMilliseconds">
        /// The inclusive upper bound of the randomized duration, measured in 
        /// milliseconds (1000ms = 1s).
        /// </param>
        IExecutionDurationRandomizerScope Create(int minDurationInMilliseconds, int maxDurationInMilliseconds);
    }
}
