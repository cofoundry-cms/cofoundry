using Cofoundry.Domain.Extendable;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PasswordPolicyBuilder : IExtendablePasswordPolicyBuilder
{
    Dictionary<Type, OptionsConfigurationAction> _descriptors = new Dictionary<Type, OptionsConfigurationAction>();
    Dictionary<string, string> _attributes = new Dictionary<string, string>();
    private string _description = null;

    public PasswordPolicyBuilder(
        IServiceProvider serviceProvider,
        PasswordOptions options
        )
    {
        ServiceProvider = serviceProvider;
        Options = options;
    }

    public IServiceProvider ServiceProvider { get; }

    public PasswordOptions Options { get; }

    public IPasswordPolicyBuilder SetDescription(string description)
    {
        _description = description;

        return this;
    }

    public IPasswordPolicyBuilder AddAttribute(string attribute, string value)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(attribute);

        if (value != null)
        {
            _attributes[attribute] = value;
        }
        else if (_attributes.ContainsKey(attribute))
        {
            _attributes.Remove(attribute);
        }

        return this;
    }

    public IPasswordPolicyBuilder RemoveAttribute(string attribute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(attribute);

        if (_attributes.ContainsKey(attribute))
        {
            _attributes.Remove(attribute);
        }

        return this;
    }

    public IPasswordPolicyBuilder AddValidator<TValidator>()
        where TValidator : INewPasswordValidatorBase
    {
        _descriptors[typeof(TValidator)] = null;
        return this;
    }

    public IPasswordPolicyBuilder RemoveValidator<TValidator>() where TValidator : INewPasswordValidatorBase
    {
        var type = typeof(TValidator);
        if (_descriptors.ContainsKey(type))
        {
            _descriptors.Remove(type);
        }

        return this;
    }

    public IPasswordPolicyBuilder AddValidatorWithConfig<TValidator, TOptions>(TOptions options)
        where TValidator : INewPasswordValidatorWithConfig<TOptions>
    {
        _descriptors[typeof(TValidator)] = new OptionsConfigurationAction<TOptions>(options);
        return this;
    }

    /// <summary>
    /// Builds the policy into a new <see cref="IPasswordPolicy"/> instance. Building the policy
    /// involves resolving validators from DI and so the lifetime of an <see cref="IPasswordPolicy"/> 
    /// should be short.
    /// </summary>
    /// <returns>A new <see cref="IPasswordPolicy"/> instance.</returns>
    public IPasswordPolicy Build()
    {
        var validators = _descriptors
            .Select(d => CreateValidator(d.Key, d.Value))
            .ToArray();

        return new PasswordPolicy(_description, validators, _attributes);
    }

    private INewPasswordValidatorBase CreateValidator(Type validatorType, OptionsConfigurationAction options)
    {
        var instance = ServiceProvider.GetRequiredService(validatorType) as INewPasswordValidatorBase;
        options?.Configure(instance);

        return instance;
    }

    private abstract class OptionsConfigurationAction
    {
        public abstract void Configure(INewPasswordValidatorBase newPasswordValidator);
    }

    private class OptionsConfigurationAction<TOptions> : OptionsConfigurationAction
    {
        private readonly TOptions _options;

        public OptionsConfigurationAction(TOptions options)
        {
            _options = options;
        }

        public override void Configure(INewPasswordValidatorBase newPasswordValidator)
        {
            var newPasswordValidatorWithConfig = newPasswordValidator as INewPasswordValidatorWithConfig<TOptions>;
            if (newPasswordValidatorWithConfig == null)
            {
                throw new InvalidOperationException($"{nameof(newPasswordValidator)} should be castable to {nameof(INewPasswordValidatorWithConfig<TOptions>)} but cast returned null.");
            }

            newPasswordValidatorWithConfig.Configure(_options);
        }
    }
}
