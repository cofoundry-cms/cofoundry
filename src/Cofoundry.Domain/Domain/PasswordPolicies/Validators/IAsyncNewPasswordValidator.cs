using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// A validator that validates a single password criteria, requiring
/// an asynchronous call e.g. for an external service, database or IO access.
/// For synchronous validators use <see cref="INewPasswordValidator"/>.
/// </para>
/// <para>
/// Asynchronous validators are only called if all other validators succeed, this
/// is to improve performance and ensure that external services etc are not called 
/// uneccessarily.
/// </para>
/// </summary>
public interface IAsyncNewPasswordValidator : INewPasswordValidatorBase
{
    /// <summary>
    /// Validates a new password request against the criteria of this validator.
    /// If validation was unsuccessful a <see cref="ValidationError"/> is retured;
    /// otherwise <see langword="null"/> is returned indicating success..
    /// </summary>
    /// <param name="context">
    /// A context object that contains the new password and other data about the request that may
    /// be useful for validating the password.
    /// </param>
    Task<ValidationError> ValidateAsync(INewPasswordValidationContext context);
}
