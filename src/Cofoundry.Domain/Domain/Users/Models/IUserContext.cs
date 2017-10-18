using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a users connection to the system at a specific point in time.
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Gets an identifier for the user either using the
        /// UserId if it exists or the IP Address if it is an annonymous user.
        /// </summary>
        /// <returns>A string identifier for the user.</returns>
        int? UserId { get; set; }

        /// <summary>
        /// If the user is logged in this indicates which User Area they are logged into. Typically
        /// the only user area will be Cofoundry Admin, but some sites may have additional custom user areas
        /// e.g. a members area.
        /// </summary>
        IUserAreaDefinition UserArea { get; set; }

        /// <summary>
        /// Indicates if the user should be required to change thier password when they log on.
        /// </summary>
        bool IsPasswordChangeRequired { get; set; }

        /// <summary>
        /// The role that this user belongs to. If this is null then the anonymous role will be used.
        /// </summary>
        int? RoleId { get; set; }

        /// <summary>
        /// Optional role code for the role this user belongs to. The role code indicates that the role
        /// is a code-first role.
        /// </summary>
        string RoleCode { get; set; }

        /// <summary>
        /// Indicated if the user belongs to the Cofoundry user area.
        /// </summary>
        bool IsCofoundryUser();
    }
}
