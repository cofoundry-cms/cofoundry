using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns the password policy for a user area.
    /// </summary>
    public class GetPasswordPolicyDescriptionByUserAreaCodeQuery : IQuery<PasswordPolicyDescription>
    {
        /// <summary>
        /// Initislized a new <see cref="GetPasswordPolicyDescriptionByUserAreaCodeQuery"/> instance.
        /// </summary>
        public GetPasswordPolicyDescriptionByUserAreaCodeQuery() { }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to get the 
        /// policy for. If the user area does not exist then an exception will be thrown.
        /// </param>
        public GetPasswordPolicyDescriptionByUserAreaCodeQuery(string userAreaCode)
        {
            UserAreaCode = userAreaCode;
        }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to get the 
        /// policy for. If the usera re adoes not exist then an exception will be thrown.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
