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
        /// Get the connection context of the current user. By default the UserContext
        /// is cached for the lifetime of the service (per request in web scenarios).
        /// </summary>
        Task<IUserContext> GetCurrentContextAsync();

        /// <summary>
        /// Use this to get a user context for the system user, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges.
        /// </summary>
        Task<IUserContext> GetSystemUserContextAsync();

        /// <summary>
        /// Get the connection context of the current user for a specific 
        /// user area. This can be useful in multi-userarea sites where the ambient
        /// context may not be the context you need.
        /// </summary>
        /// <remarks>
        /// In Cofoundry we use this to get the user context of a logged in Cofoundry
        /// user for the site viewer, irrespective of the ambient user context, which 
        /// potentially could be (for example) a members area.
        /// </remarks>
        Task<IUserContext> GetCurrentContextByUserAreaAsync(string userAreaCode);

        /// <summary>
        /// Clears out the cached user context if one exists. Typically the user 
        /// context is cached for the duration of the request so it needs clearing if
        /// it changes (i.e. logged in or out).
        /// </summary>
        void ClearCache();
    }
}
