using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if an account recovery request is valid. The result is returned as a 
    /// <see cref="ValidationQueryResult"/> which describes any errors that have occured.
    /// </summary>
    public class ValidateUserAccountRecoveryRequestQuery : IQuery<ValidationQueryResult>
    {
        /// <summary>
        /// A token used to identify and authenticate the request when resetting the password.
        /// May be <see langword="null"/> if the token was not present in the querystring.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
