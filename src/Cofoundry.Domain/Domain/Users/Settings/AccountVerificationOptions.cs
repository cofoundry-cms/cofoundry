using Cofoundry.Core.Validation;
using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options to control the behavior of the account verification feature.
    /// Note that the Cofoundry admin panel does not support an account 
    /// verification flow and therefore these settings do not apply.
    /// </summary>
    public class AccountVerificationOptions : IValidatableObject
    {
        /// <summary>
        /// If set to <see langword="true"/>, then an account is required to be
        /// verified before being able to log in. Defaults to <see langword="false"/>.
        /// The Cofoundry Admin panel does not support account verification.
        /// </summary>
        public bool RequireVerification { get; set; }

        /// <summary>
        /// The length of time an account verification token is valid for, specified as a 
        /// <see cref="TimeSpan"/> or in JSON configuration as a time format string 
        /// e.g. "01:00:00" to represent 1 hour. Defaults to 7 days. If zero or
        /// less, then time-based validation does not occur.
        /// </summary>
        public TimeSpan ExpireAfter { get; set; } = TimeSpan.FromDays(7);

        /// <summary>
        /// The maximum number of account verification initiation attempts to allow per IP address. The 
        /// default value is 16 attempts per day.
        /// </summary>
        [ValidateObject]
        public RateLimitConfiguration InitiationRateLimit { get; set; } = new RateLimitConfiguration(16, TimeSpan.FromDays(1));

        /// <summary>
        /// <para>
        /// The relative base path used to construct the URL for the account verification completion
        /// page. A unique token will be added as a query parameter to the URL, it is then
        /// resolved using <see cref="ISiteUrlResolver.MakeAbsolute(string)"/> and added to the 
        /// email notification e.g. "/auth/account/verify" would be transformed to 
        /// "https://example.com/auth/account/verify?t={token}". The path can include
        /// other query parameters, which will be merged into the resulting url.
        /// </para>
        /// <para>
        /// This setting is required when using the account verification feature, unless you are building
        /// the url yourself in a custom <see cref="MailTemplates.DefaultMailTemplates.IDefaultMailTemplateBuilder{T}"/> 
        /// implementation.
        /// </para>
        /// </summary>
        public string VerificationUrlBase { get; set; }

        /// <summary>
        /// Copies the options to a new instance, which can be modified
        /// without altering the base settings. This is used for user area
        /// specific configuration.
        /// </summary>
        public AccountVerificationOptions Clone()
        {
            return new AccountVerificationOptions()
            {
                RequireVerification = RequireVerification,
                ExpireAfter = ExpireAfter,
                InitiationRateLimit = InitiationRateLimit.Clone(),
                VerificationUrlBase = VerificationUrlBase
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VerificationUrlBase != null && !Uri.IsWellFormedUriString(VerificationUrlBase, UriKind.Relative))
            {
                yield return new ValidationResult($"{VerificationUrlBase} must be a relative url.", new string[] { nameof(VerificationUrlBase) });
            }
        }
    }
}