using Cofoundry.Core.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.ExecutionDurationRandomizer
{
    /// <summary>
    /// The duration parameters of an <see cref="IExecutionDurationRandomizerScope"/>.
    /// </summary>
    /// <inheritdoc/>
    public class RandomizedExecutionDuration : IValidatableObject, IFeatureEnableable
    {
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The inclusive lower bound of the randomized duration, measured in 
        /// milliseconds (1000ms = 1s). Set to <see langword="null"/> to disable the feature.
        /// </summary>
        public int? MinInMilliseconds { get; set; }

        /// <summary>
        /// The inclusive upper bound of the randomized duration, measured in 
        /// milliseconds (2000ms = 2s). Set to zero or <see langword="null"/> to disable the feature.
        /// </summary>
        public int? MaxInMilliseconds { get; set; }

        /// <summary>
        /// <see langword="true"/> if <see cref="Enabled"/> is <see langword="true"/>
        /// and the parameters are configured to provide a valid duration.
        /// </summary>
        public bool IsEnabled()
        {
            return Enabled && MinInMilliseconds >= 0 && MaxInMilliseconds > 0;
        }

        /// <summary>
        /// <see langword="true"/> if the <see cref="MinInMilliseconds"/> and <see cref="MaxInMilliseconds"/>
        /// values are the same.
        /// </summary>
        public bool IsConstant()
        {
            return MinInMilliseconds == MaxInMilliseconds;
        }

        /// <summary>
        /// Updates this instance based on the values in <paramref name="newDuration"/>.
        /// If this instance is not enabled then all values will be replaced with those 
        /// in <paramref name="newDuration"/>.
        /// </summary>
        /// <param name="newDuration">The duration containing updated values to copy from.</param>
        public void Update(RandomizedExecutionDuration newDuration)
        {
            // if duration not enabled, then there is nothing to update
            if (newDuration == null || !newDuration.IsEnabled()) return;


            if (!Enabled)
            {
                // If this duration is disabled, then we should update
                // all values
                Enabled = true;
                MinInMilliseconds = newDuration.MinInMilliseconds.Value;
                MaxInMilliseconds = newDuration.MaxInMilliseconds.Value;
            }

            if (newDuration.MinInMilliseconds > (MinInMilliseconds ?? 0))
            {
                MinInMilliseconds = newDuration.MinInMilliseconds.Value;
            }

            if (newDuration.MaxInMilliseconds > (MaxInMilliseconds ?? 0))
            {
                MaxInMilliseconds = newDuration.MaxInMilliseconds.Value;
            }
        }

        /// <summary>
        /// Creates a clone of this instance, copying all values to
        /// a new <see cref="RandomizedExecutionDuration"/> instance.
        /// </summary>
        /// <returns></returns>
        public RandomizedExecutionDuration Clone()
        {
            return new RandomizedExecutionDuration()
            {
                Enabled = Enabled,
                MaxInMilliseconds = MaxInMilliseconds,
                MinInMilliseconds = MinInMilliseconds
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinInMilliseconds.HasValue && MinInMilliseconds < 0)
            {
                yield return new ValidationResult("Minimum duration cannot be less than zero.", new string[] { nameof(MinInMilliseconds) });
            }

            if ((MaxInMilliseconds ?? 0) < (MinInMilliseconds ?? 0))
            {
                yield return new ValidationResult("Maximum duration cannot be less than minimum duration.", new string[] { nameof(MaxInMilliseconds) });
            }
        }
    }
}