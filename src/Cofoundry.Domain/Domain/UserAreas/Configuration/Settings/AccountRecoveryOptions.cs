using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options to control the behavior of the self-service account recovery feature.
    /// </summary>
    public class AccountRecoveryOptions : IValidatableObject
    {
        /// <summary>
        /// The length of time an account recovery token is valid for, specified as a 
        /// <see cref="TimeSpan"/> or in JSON configuration as a time format string 
        /// e.g. "01:00:00" to represent 1 hour. Defaults to 16 hours. If zero or
        /// less, then time-based validation does not occur.
        /// </summary>
        public TimeSpan ExpireAfter { get; set; } = TimeSpan.FromHours(16);

        /// <summary>
        /// The maximum number of account recovery attempts to allow within the
        /// given <see cref="RateLimitWindow"/>. Defaults to 16 attempts. If zero 
        /// or less, then max attempt validation does not occur.
        /// </summary>
        public int RateLimitQuantity { get; set; } = 16;

        /// <summary>
        /// The time-window in which to count account recovery attempts when enforcing
        /// <see cref="RateLimitQuantity"/> validation, specified as a <see cref="TimeSpan"/> 
        /// or in JSON configuration as a time format string e.g. "01:00:00" to represent 
        /// 1 hour. Defaults to 24 hours. If zero or less, then max attempt validation does not occur.
        /// </summary>
        public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// <para>
        /// The relative base path used to construct the URL for the account recovery completion
        /// form. A unique token will be added as a query parameter to the URL, it is then
        /// resolved using <see cref="ISiteUrlResolver.MakeAbsolute(string)"/> and added to the 
        /// email notification e.g. "/auth/account-recovery/complete" would be transformed to 
        /// "https://example.com/auth/account-recovery/complete?t={token}". The path can include
        /// other query parameters, which will be merged into the resulting url.
        /// </para>
        /// <para>
        /// This setting is required when using the account recovery feature, unless you are building
        /// the url yourself in a custom <see cref="MailTemplates.DefaultMailTemplates.IDefaultMailTemplateBuilder{T}"/> 
        /// implementation. Changing this setting does not affect the Cofoundry Admin account recovery feature.
        /// </para>
        /// </summary>
        public string RecoveryUrlBase { get; set; }

        /// <summary>
        /// Copies the options to a new instance, which can be modified
        /// without altering the base settings. This is used for user area
        /// specific configuration.
        /// </summary>
        public AccountRecoveryOptions Clone()
        {
            return new AccountRecoveryOptions()
            {
                ExpireAfter = ExpireAfter,
                RateLimitQuantity = RateLimitQuantity,
                RateLimitWindow = RateLimitWindow,
                RecoveryUrlBase = RecoveryUrlBase
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RecoveryUrlBase != null && !Uri.IsWellFormedUriString(RecoveryUrlBase, UriKind.Relative))
            {
                yield return new ValidationResult($"{RecoveryUrlBase} must be a relative url.", new string[] { nameof(RecoveryUrlBase) });
            }
        }
    }
}
