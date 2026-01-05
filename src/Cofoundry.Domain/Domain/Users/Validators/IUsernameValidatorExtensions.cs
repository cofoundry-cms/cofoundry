namespace Cofoundry.Domain;

public static class IUsernameValidatorExtensions
{
    extension(IUsernameValidator usernameValidator)
    {
        /// <summary>
        /// Validates a username, throwing a <see cref="ValidationErrorException"/>
        /// if any errors are found.  By default the validator checks that the 
        /// format contains only the characters permitted by the  <see cref="UsernameOptions"/> 
        /// configuration settings, as well as checking for uniquness.
        /// </summary>
        public async Task ValidateAsync(IUsernameValidationContext context)
        {
            var result = await usernameValidator.GetErrorsAsync(context);

            if (result.Count != 0)
            {
                throw new ValidationErrorException(result.First());
            }
        }
    }
}
