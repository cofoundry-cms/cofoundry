namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PasswordPolicyService : IPasswordPolicyService
{
    private readonly IPasswordPolicyFactory _passwordPolicyConfigurationFactory;

    public PasswordPolicyService(
        IPasswordPolicyFactory passwordPolicyConfigurationFactory
        )
    {
        _passwordPolicyConfigurationFactory = passwordPolicyConfigurationFactory;
    }

    public virtual PasswordPolicyDescription GetDescription(string userAreaCode)
    {
        var passwordPolicy = _passwordPolicyConfigurationFactory.Create(userAreaCode);
        var criteria = passwordPolicy.GetCriteria().ToArray();

        return new PasswordPolicyDescription()
        {
            Description = passwordPolicy.Description,
            Criteria = criteria,
            Attributes = passwordPolicy.Attributes
        };
    }

    public virtual async Task ValidateAsync(INewPasswordValidationContext context)
    {
        ValidateContext(context);
        var passwordPolicy = _passwordPolicyConfigurationFactory.Create(context.UserAreaCode);
        var errors = await passwordPolicy.ValidateAsync(context);

        if (errors.Any())
        {
            throw new ValidationErrorException(errors.First());
        }
    }

    private static void ValidateContext(INewPasswordValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrEmpty(context.Password))
        {
            throw new ArgumentException("Cannot validate a null or empty password value.");
        }

        if (string.IsNullOrEmpty(context.UserAreaCode))
        {
            throw new ArgumentException($"{nameof(INewPasswordValidationContext)} must supply a {nameof(INewPasswordValidationContext.UserAreaCode)}.");
        }
    }
}
