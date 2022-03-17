namespace Cofoundry.Domain;

/// <summary>
/// Initiates a user account verification flow whereby validation is performed
/// using a unique string token, which is typically inserted into a link in an email
/// notification.
/// </summary>
/// <remarks>
/// This command utilises the "Authorized Task" framework to do most of the work, and if
/// you want a more customized process then you may wish to use the authorized task framework 
/// directly.
/// </remarks>
public class CompleteUserAccountVerificationViaEmailCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user belongs to.
    /// </summary>
    [Required]
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The token used to verify the request.
    /// </summary>
    [Required]
    public string Token { get; set; }
}
