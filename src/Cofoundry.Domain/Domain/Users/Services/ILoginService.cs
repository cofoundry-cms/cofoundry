using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for logging users in and out of the application.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Logs a user into the application but performs no 
        /// authentication. The user should have already passed 
        /// authentication prior to calling this method.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area to log into.</param>
        /// <param name="userId">The id of the user to log in.</param>
        /// <param name="rememberUser">
        /// True if the user should stay logged in perminantely; false
        /// if the user should only stay logged in for the duration of
        /// the session.
        /// </param>
        Task LogAuthenticatedUserInAsync(string userAreaCode, int userId, bool rememberUser);

        /// <summary>
        /// Signs the user out of the application and ends the session.
        /// </summary>
        /// <param name="userAreaCode">The code of the user area to log out of.</param>
        Task SignOutAsync(string userAreaCode);

        /// <summary>
        /// Signs the user out of all user areas and ends the session.
        /// </summary>
        Task SignOutAllUserAreasAsync();
    }
}
