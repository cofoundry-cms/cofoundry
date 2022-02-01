using Cofoundry.Domain.MailTemplates.DefaultMailTemplates;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used to format 
    /// an account verification (AKA confirm account) mail template.
    /// </summary>
    public class AccountVerificationTemplateBuilderContext
    {
        /// <summary>
        /// The user to verify.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// A token used to identify and authenticate when verifying the account. This
        /// token has been used to create the <see cref="Verification"/> property,
        /// but is provided separately here so you can rebuild your own URL if required.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The relative URL for the account verification completion form including the token 
        /// parameter e.g. "/auth/account/verify?t=example-token". Generally  this should 
        /// not be <see langword="null"/> because it should be configured in <see cref="AccountVerificationOptions.VerificationUrlBase"/>,
        /// however, it is possible to be <see langword="null"/> if this setting was not 
        /// set or the URL building is handled in a custom <see cref="IDefaultMailTemplateBuilder{T}"/> 
        /// implementation.
        /// </summary>
        public string VerificationUrlPath { get; set; }
    }
}