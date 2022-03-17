namespace Cofoundry.Core.ExecutionDurationRandomizer;

/// <summary>
/// A scope that controls the duration of a task as part of the execution 
/// duration randomizer system. When the scope coompletes, any extra
/// time that needs to be added to the duration is padded using <see cref="Task.Delay"/>.
/// Scopes are managed by <see cref="IExecutionDurationRandomizerScopeManager"/>
/// which is the main public API for the feature.
/// </summary>
public interface IExecutionDurationRandomizerScope : IAsyncDisposable
{
    /// <summary>
    /// Completes the scope, padding out any remaining time to meet
    /// the expected execution duration.
    /// </summary>
    Task CompleteAsync();
}
