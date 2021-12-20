using Cofoundry.Domain.CQS;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if a password reset request is valid. The reslt is returned as a 
    /// <see cref="PasswordResetRequestAuthenticationResult"/> which uses 
    /// <see cref="PasswordResetRequestAuthenticationError"/> to describe specific types of error
    /// that can occur.
    /// </summary>
    public class ValidatePasswordResetRequestQuery : IQuery<PasswordResetRequestAuthenticationResult>
    {
        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password. May be <see cref="Guid.Empty"/> 
        /// if the value could not be parsed correctly.
        /// </summary>
        [Required]
        public Guid UserPasswordResetRequestId { get; set; }

        /// <summary>
        /// A token used to authenticate when resetting the password.
        /// May be null if the token was not present in the querystring.
        /// </summary>
        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
