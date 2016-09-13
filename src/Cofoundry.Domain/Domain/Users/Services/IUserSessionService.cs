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
        int? GetCurrentUserId();

        void SetCurrentUserId(int userId, bool rememberUser);

        /// <summary>
        /// Abandons the current session and removes the users
        /// login cookie
        /// </summary>
        void Abandon();
    }
}
