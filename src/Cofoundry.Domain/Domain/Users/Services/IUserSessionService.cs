using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service to abstract away the management of a users current browser session and 
    /// authentication cookie.
    /// </summary>
    public interface IUserSessionService
    {
        /// <summary>
        /// Gets the UserId of the user currently logged
        /// in to this session
        /// </summary>
        /// <returns>
        /// UserId of the user currently logged
        /// in to this session
        /// </returns>
        int? GetCurrentUserId();

        /// <summary>
        /// Assigns the specified UserId to the current session.
        /// </summary>
        /// <param name="userId">UserId belonging to the owner of the current session.</param>
        /// <param name="rememberUser">
        /// True if the session should last indefinately; false if the 
        /// session should close after a timeout period.
        /// </param>
        Task SetCurrentUserIdAsync(int userId, bool rememberUser);

        /// <summary>
        /// Abandons the current session and removes the users
        /// login cookie
        /// </summary>
        Task AbandonAsync();
    }
}
