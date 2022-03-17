using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// A new password validator that can be configured with a set
/// of options.
/// </summary>
/// <typeparam name="TOptions">
/// The type of options to be supplied to the <see cref="Configure"/>
/// method.
/// </typeparam>
public interface INewPasswordValidatorWithConfig<TOptions> : INewPasswordValidatorBase
{
    /// <summary>
    /// Configures the validator with the specified <paramref name="options"/>.
    /// This is expected to be called after constructions but before validation.
    /// </summary>
    /// <param name="options">
    /// The options to configure the validator with.
    /// </param>
    void Configure(TOptions options);
}
