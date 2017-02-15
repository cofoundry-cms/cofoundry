using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for retreiving user connection information.
    /// </summary>
    public interface IUserContextService
    {
        /// <summary>
        /// Get the connection context of the current user.
        /// </summary>
        IUserContext GetCurrentContext();

        /// <summary>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges.
        /// </summary>
        Task<IUserContext> GetSystemUserContextAsync();

        /// <summary>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges.
        /// </summary>
        IUserContext GetSystemUserContext();

        /// <summary>
        /// Clears out the cached user context if one exists. Typically the user 
        /// context is cached for the duration of the request so it needs clearing if
        /// it changes (i.e. logged in or out).
        /// </summary>
        void ClearCache();
    }
}
