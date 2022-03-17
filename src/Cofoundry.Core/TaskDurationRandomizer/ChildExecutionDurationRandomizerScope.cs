namespace Cofoundry.Core.ExecutionDurationRandomizer.Internal;

/// <summary>
/// A nested scope where the handling of the scope has been deferred to a
/// primary <see cref="ExecutionDurationRandomizerScope"/> instance.
/// </summary>
/// <inheritdoc/>
public class ChildExecutionDurationRandomizerScope : IExecutionDurationRandomizerScope
{
    private bool _isCompleteCalled = false;

    public Task CompleteAsync()
    {
        if (_isCompleteCalled)
        {
            throw new InvalidOperationException($"{nameof(ExecutionDurationRandomizerScope)}.{nameof(CompleteAsync)} cannot be called twice.");
        }

        _isCompleteCalled = true;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isCompleteCalled)
        {
            await CompleteAsync();
        }
    }
}
