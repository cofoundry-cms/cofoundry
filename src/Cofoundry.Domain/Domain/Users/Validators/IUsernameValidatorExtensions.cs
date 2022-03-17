namespace Cofoundry.Domain;

public static class IUsernameValidatorExtensions
{
    /// <summary>
    /// Validates a username, throwing a <see cref="ValidationErrorException"/>
    /// if any errors are found.  By default the validator checks that the 
    /// format contains only the characters permitted by the  <see cref="UsernameOptions"/> 
    /// configuration settings, as well as checking for uniquness.
    /// </summary>
    public static async Task ValidateAsync(this IUsernameValidator usernameValidator, IUsernameValidationContext context)
    {
        var result = await usernameValidator.GetErrorsAsync(context);

        if (result.Any())
        {
            throw new ValidationErrorException(result.First());
        }
    }
}
