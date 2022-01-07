using Cofoundry.Core.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class IEmailAddressValidatorExtensions
    {
        /// <summary>
        /// Validates a user email address, throwing a <see cref="ValidationErrorException"/>
        /// if any errors are found. By default the validator checks that the format contains only 
        /// the characters permitted by the <see cref="EmailAddressOptions"/> configuration settings, 
        /// as well as checking for uniquness.
        /// </summary>
        public static async Task ValidateAsync(this IEmailAddressValidator emailAddressValidator, IEmailAddressValidationContext context)
        {
            var result = await emailAddressValidator.GetErrorsAsync(context);

            if (result.Any())
            {
                throw new ValidationErrorException(result.First());
            }
        }
    }
}