namespace Cofoundry.Core.ExecutionDurationRandomizer.Internal;

/// <inheritdoc/>
public class ExecutionDurationRandomizerScopeManager : IExecutionDurationRandomizerScopeManager
{
    private readonly ExecutionDurationRandomizerSettings _executionDurationRandomizerSettings;

    public ExecutionDurationRandomizerScopeManager(
        ExecutionDurationRandomizerSettings executionDurationRandomizerSettings
        )
    {
        _executionDurationRandomizerSettings = executionDurationRandomizerSettings;
    }

    private ExecutionDurationRandomizerScope _executionDurationRandomizerScope = null;

    public IExecutionDurationRandomizerScope Create(
        int minDurationInMilliseconds,
        int maxDurationInMilliseconds
        )
    {
        return Create(new RandomizedExecutionDuration()
        {
            Enabled = true,
            MinInMilliseconds = minDurationInMilliseconds,
            MaxInMilliseconds = maxDurationInMilliseconds
        });
    }

    public IExecutionDurationRandomizerScope Create(RandomizedExecutionDuration duration)
    {
        if (_executionDurationRandomizerScope == null)
        {
            _executionDurationRandomizerScope = new ExecutionDurationRandomizerScope(_executionDurationRandomizerSettings);
            _executionDurationRandomizerScope.UpdateDuration(duration);
        }
        else
        {
            _executionDurationRandomizerScope.UpdateDuration(duration);
            return new ChildExecutionDurationRandomizerScope();
        }

        return _executionDurationRandomizerScope;
    }
}
