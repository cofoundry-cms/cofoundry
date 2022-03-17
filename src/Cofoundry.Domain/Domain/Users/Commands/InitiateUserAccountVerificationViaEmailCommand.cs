namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// Initiates an email-based account verification (AKA "confirm account") request, sending an 
/// email notification to the user with a url to an account verification page. 
/// </para>
/// <para>
/// This command utilises the "Authorized Task" framework to do most of the work, and if
/// you want a more customized process then you may wish to use the authorized task framework 
/// directly.
/// </para>
/// </summary>
public class InitiateUserAccountVerificationViaEmailCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// The id of the user to initiate the account verification process with.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int UserId { get; set; }
}
