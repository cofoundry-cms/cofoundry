using Cofoundry.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to validate an email address when adding or updating a user.
    /// </summary>
    public interface IEmailAddressValidator
    {
        /// <summary>
        /// Validates a user email address, returning any errors found. By default the validator
        /// checks that the format contains only the characters permitted by the 
        /// <see cref="EmailAddressOptions"/> configuration settings, as well as checking
        /// for uniqueness if necessary.
        /// </summary>
        Task<ICollection<ValidationError>> GetErrorsAsync(IEmailAddressValidationContext context);
    }
}