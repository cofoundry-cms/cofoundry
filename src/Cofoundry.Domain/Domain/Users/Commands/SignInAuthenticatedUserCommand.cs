namespace Cofoundry.Domain;

/// <summary>
/// Signs in a user that has already passed an authentication check. The user 
/// should have already passed authentication prior to calling this method. The 
/// ambient user area (i.e. "current" user context) is switched to the specified area 
/// for the remainder of the DI scope (i.e. request for web apps).
/// </summary>
public class SignInAuthenticatedUserCommand : ICommand
{
    /// <summary>
    /// Database id of the user to sign in.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int UserId { get; set; }

    /// <summary>
    /// <see langword="true"/> if the user should stay logged in perminantely; 
    /// <see langword="false"/> if the user should only stay logged in for the 
    /// duration of the browser session.
    /// </summary>
    public bool RememberUser { get; set; }
}
