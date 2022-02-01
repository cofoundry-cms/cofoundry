using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if an authorized task token is valid. The result is returned as a 
    /// <see cref="AuthorizedTaskTokenValidationResult"/> which describes any errors that have occured.
    /// </summary>
    public class ValidateAuthorizedTaskTokenQuery : IQuery<AuthorizedTaskTokenValidationResult>
    {
        /// <summary>
        /// The <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/> that this task is
        /// expected to be categorized as.
        /// </summary>
        [Required]
        public string AuthorizedTaskTypeCode { get; set; }

        /// <summary>
        /// A token used to identify and authenticate a task before it is executed.
        /// May be <see langword="null"/> if the token was not present in the request.
        /// </summary>
        public string Token { get; set; }
    }
}