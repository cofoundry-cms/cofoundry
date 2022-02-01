using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options used in the default password validation logic.
    /// </summary>
    public class PasswordOptions : IValidatableObject
    {
        public const int MIN_LENGTH_BOUNDARY = 6;
        public const int MAX_LENGTH_BOUNDARY = 2048;

        /// <summary>
        /// The minimum length of a password. Defaults to 10 and
        /// anything less is not recommended. Must be between 6 and 2048 characters.
        /// </summary>
        [Range(MIN_LENGTH_BOUNDARY, MAX_LENGTH_BOUNDARY)]
        public int MinLength { get; set; } = 10;

        /// <summary>
        /// The maximum length of a password. Defaults to 300 characters and must be between 6 and 2048 characters.
        /// </summary>
        [Range(MIN_LENGTH_BOUNDARY, MAX_LENGTH_BOUNDARY)]
        public int MaxLength { get; set; } = 300;

        /// <summary>
        /// The number of unique characters required in a password. This is
        /// to prevent passwords like "aabbccdd". Defaults to 5 unique
        /// characters.
        /// </summary>
        [Range(1, MAX_LENGTH_BOUNDARY)]
        public int MinUniqueCharacters { get; set; } = 5;

        /// <summary>
        /// Indicates whether to send a confirmation notification to the user to let them
        /// know their password has been changed. This only applied when a password is changed
        /// by the user and not via a reset e.g. via <see cref="UpdateCurrentUserPasswordCommand"/>
        /// or <see cref="CompleteUserAccountRecoveryByEmailCommand"/>. Defaults to <see langword="true"/>.
        /// </summary>
        public bool SendNotificationOnUpdate { get; set; } = true;

        /// <summary>
        /// Clones the options, copying data over to a new <see cref="PasswordOptions"/>
        /// instance.
        /// </summary>
        public PasswordOptions Clone()
        {
            return new PasswordOptions()
            {
                MaxLength = MaxLength,
                MinLength = MinLength,
                MinUniqueCharacters = MinUniqueCharacters,
                SendNotificationOnUpdate = SendNotificationOnUpdate
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinLength > MaxLength)
            {
                yield return new ValidationResult(
                    $"{nameof(MinLength)} cannot be longer then {nameof(MaxLength)}", 
                    new string[] { nameof(MinLength), nameof(MaxLength) }
                    );
            }
        }
    }
}
