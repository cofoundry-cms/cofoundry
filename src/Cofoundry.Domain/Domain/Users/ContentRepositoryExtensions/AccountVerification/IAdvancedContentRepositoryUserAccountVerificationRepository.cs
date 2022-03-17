namespace Cofoundry.Domain;

/// <summary>
/// Queries and commands for account verification.
/// </summary>
public interface IAdvancedContentRepositoryUserAccountVerificationRepository
{
    /// <summary>
    /// Sets the account verification status of the user. Account 
    /// verification is a generic flag to mark a user as verified
    /// or activated. This command isn't concerned with how the
    /// verification has happened, but this is often done via an
    /// email notification or another out-of-band communication
    /// with a verification code.
    /// </summary>
    Task UpdateStatusAsync(UpdateUserAccountVerificationStatusCommand command);

    /// <summary>
    /// The email-based verification flow performs the most common account
    /// validation flow by sending an email with a verification link.
    /// </summary>
    IAdvancedContentRepositoryUserAccountVerificationByEmailRepository EmailFlow();
}
