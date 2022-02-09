using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Signs the user out of all user areas and ends the session. A
    /// <see cref="UserSignedOutMessage"/> is published for each user
    /// area that is logged out.
    /// </summary>
    public class SignOutCurrentUserFromAllUserAreasCommand : ICommand, ILoggableCommand
    {
    }
}