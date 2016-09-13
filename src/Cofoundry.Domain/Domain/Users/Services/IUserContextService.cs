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
        /// Use this to get a user context for the super admin, useful
        /// if you need to impersonate the user to perform an action with elevated 
        /// privileges
        /// </summary>
        Task<IUserContext> GetSuperAdminUserContextAsync();

        IUserContext GetSuperAdminUserContext();

        void ClearCache();
    }
}
