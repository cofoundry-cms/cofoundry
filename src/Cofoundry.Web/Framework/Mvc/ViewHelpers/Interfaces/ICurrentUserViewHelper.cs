using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// A view helper for providing information about the currently logged in user
    /// </summary>
    public interface ICurrentUserViewHelper
    {
        /// <summary>
        /// Indicates whether the user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Indicates whether the user is logged in and is a user of the Cofoundry admin area. The user
        /// table may be used by non-cofoundry users too so this differentiates them.
        /// </summary>
        bool IsCofoundryUser { get; }

        /// <summary>
        /// The context of the currently logged in user
        /// </summary>
        IUserContext Context { get; }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        UserMicroSummary User { get; }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        RoleDetails Role { get; }
    }
}
