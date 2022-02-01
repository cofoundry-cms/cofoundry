using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Sets the account verification status of the user. Account 
    /// verification is a generic flag to mark a user as verified
    /// or activated. This command isn't concerned with how the
    /// verification has happened, but this is often done via an
    /// email notification or another out-of-band communication
    /// with a verification code.
    /// </summary>
    public class UpdateUserAccountVerificationStatusCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the user to set the verification status of.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        /// <summary>
        /// Indicates whether the user account should be marked as verified.
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public bool IsAccountVerified { get; set; }
    }
}