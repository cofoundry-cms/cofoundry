using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cofoundry.Core.ExecutionDurationRandomizer
{
    /// <inheritdoc/>
    public class ExecutionDurationRandomizerScope : IExecutionDurationRandomizerScope
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ExecutionDurationRandomizerSettings _executionDurationRandomizerSettings;
        private bool _isCompleteCalled = false;

        public ExecutionDurationRandomizerScope(
            ExecutionDurationRandomizerSettings executionDurationRandomizerSettings
            )
        {
            _executionDurationRandomizerSettings = executionDurationRandomizerSettings;
        }

        public ExecutionDurationRandomizerScope(
            ExecutionDurationRandomizerSettings executionDurationRandomizerSettings,
            RandomizedExecutionDuration duration
            ) : this(executionDurationRandomizerSettings)
        {
            UpdateDuration(duration);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public RandomizedExecutionDuration Duration { get; set; }

        public void UpdateDuration(RandomizedExecutionDuration newDuration)
        {
            if (newDuration == null || !newDuration.IsEnabled()) return;

            var updatedDuration = Duration == null ? new RandomizedExecutionDuration() : Duration.Clone();
            updatedDuration.Update(newDuration);

            Duration = updatedDuration;
        }

        public async Task CompleteAsync()
        {
            if (_isCompleteCalled)
            {
                throw new InvalidOperationException($"{nameof(ExecutionDurationRandomizerScope)}.{nameof(CompleteAsync)} cannot be called twice.");
            }

            _isCompleteCalled = true;

            if (!_executionDurationRandomizerSettings.Enabled || Duration == null || !Duration.IsEnabled()) return;

            int duration;
            if (Duration.IsConstant())
            {
                duration = Duration.MaxInMilliseconds.Value;
            }
            else
            {
                duration = RandomNumberGenerator.GetInt32(Duration.MinInMilliseconds.Value, Duration.MaxInMilliseconds.Value + 1);
            }

            _stopwatch.Stop();
            var timeStillToRun = LongToInt(duration - _stopwatch.ElapsedMilliseconds);

            if (timeStillToRun > 0)
            {
                await Task.Delay(timeStillToRun);
            }
        }

        private int LongToInt(long olong)
        {
            if (olong > Int32.MaxValue)
            {
                throw new InvalidCastException("elapsed time too long");
            }

            return (int)olong;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleteCalled)
            {
                await CompleteAsync();
            }
        }
    }
}