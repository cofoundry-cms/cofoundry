namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Base marker interface for new password validators. Used
    /// so that sync and async validators can inherit from a common
    /// interface.
    /// </summary>
    public interface INewPasswordValidatorBase
    {
        /// <summary>
        /// Descriptive requirement e.g. "Must have 1 digit", "Cannot be one of your last 3 passwords"
        /// </summary>
        string Criteria { get; }

    }
}
