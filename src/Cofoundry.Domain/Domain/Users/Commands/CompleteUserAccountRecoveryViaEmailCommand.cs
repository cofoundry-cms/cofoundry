using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Completes an account recovery request initiated by
    /// <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>, updating the users
    /// password if the request is verified.
    /// </summary>
    public class CompleteUserAccountRecoveryViaEmailCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The token used to verify the request.
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The value to set as the new account password. The password will go through 
        /// additional validation depending on the password policy configuration.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }
    }
}
