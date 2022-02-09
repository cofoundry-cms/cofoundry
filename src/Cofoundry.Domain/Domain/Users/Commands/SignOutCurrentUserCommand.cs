using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Signs out the user currently logged into the "ambient" scheme, ending the session.
    /// The ambient scheme usually represents the default user area, unless it has been switched
    /// during the request. A <see cref="UserSignedOutMessage"/> is published once the user is 
    /// signed out; if the user is not signed in, no action is taken.
    /// </summary>
    public class SignOutCurrentUserCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to sign
        /// the user out of. If <see langword="null"/> then the ambient user area is used.
        /// </summary>
        public string UserAreaCode { get; set; }
    }
}