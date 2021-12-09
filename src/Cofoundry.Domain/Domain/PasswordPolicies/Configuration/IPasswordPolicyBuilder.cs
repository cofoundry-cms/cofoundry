using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to configure a password policy.
    /// </summary>
    public interface IPasswordPolicyBuilder
    {
        /// <summary>
        /// <para>
        /// Sets the description for the policy. This should be a brief description that highlights the 
        /// main criteria of the policy e.g. "Passwords must be between 10 and 300 characters.".
        /// This description is designed to be displayed to users to help them choose their new password.
        /// </para>
        /// <para>
        /// For the Cofoundry admin user area, this description is displayed when configuring a password, 
        /// but for other user areas it's use is optional and you don't need to set it if you don't use it.
        /// </para>
        /// <para>
        /// A full list of descriptive criteria is also available on <see cref="PasswordPolicyDescription.Criteria"/>
        /// but often it's better to have a more terse version available.
        /// </para>
        /// </summary>
        IPasswordPolicyBuilder SetDescription(string description);

        /// <summary>
        /// Adds an attribute to the policy. Attributes are loosely mapped to
        /// HTML attributes for password inputs. Expressing these attributes here allows
        /// a developer to dynamically map policy requirements to an input field. The
        /// <see cref="PasswordPolicyAttributes"/> constants can be used as keys for known
        /// attributes.
        /// </summary>
        /// <param name="attribute">
        /// The attribute name e.g. "minlength". The  <see cref="PasswordPolicyAttributes"/> constants 
        /// can be used for known attribute keys.
        /// </param>
        /// <param name="value">The value of the attribute as a string e.g. "10" or "minlength: 20; required: lower; required: upper;".</param>
        IPasswordPolicyBuilder AddAttribute(string attribute, string value);

        /// <summary>
        /// Removes an attribute from the policy. This is useful if you want to use the defaults
        /// but remove a specific attribute.
        /// </summary>
        /// <param name="attribute">
        /// The name of the attribute to remove e.g. "minlength". The  <see cref="PasswordPolicyAttributes"/> constants 
        /// can be used for known attribute keys.
        /// </param>
        IPasswordPolicyBuilder RemoveAttribute(string attribute);

        /// <summary>
        /// Adds the specified validator type to the policy. A validator type
        /// is only added once and repeated calls will be ignored.
        /// </summary>
        /// <typeparam name="TValidator">Type of validator to add.</typeparam>
        IPasswordPolicyBuilder AddValidator<TValidator>()
            where TValidator : INewPasswordValidatorBase;

        /// <summary>
        /// Removes the specified validator type from the policy. This is useful
        /// if you want to add the defaults and then remove one specific validator.
        /// </summary>
        /// <typeparam name="TValidator">Type of validator to remove.</typeparam>
        IPasswordPolicyBuilder RemoveValidator<TValidator>()
            where TValidator : INewPasswordValidatorBase;

        /// <summary>
        /// Adds the specified validator type and configuration options to the policy. A 
        /// validator type is only added once and repeated calls will simply overwrite
        /// the configuration.
        /// </summary>
        /// <typeparam name="TValidator">Type of validator to add.</typeparam>
        /// <typeparam name="TOptions">The type of configuration options to add with the validator.</typeparam>
        /// <param name="options">The options to configure the validator with.</param>
        IPasswordPolicyBuilder AddValidatorWithConfig<TValidator, TOptions>(TOptions options)
            where TValidator : INewPasswordValidatorWithConfig<TOptions>;
    }
}
