namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// The authentication status of a user at a specific point 
    /// in time, typically representing the current context of a user during
    /// the execution of a request. Users can log into multiple user 
    /// areas concurrently, so an <see cref="IUserContext"/> is scoped
    /// to a specific user area.
    /// </para>
    /// <para>
    /// If the user is not logged in, then most of the properties of an 
    /// <see cref="IUserContext"/> instance will be null.
    /// </para>
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Id of the User if they are logged in; otherwise <see langword="null"/>.
        /// </summary>
        int? UserId { get; set; }

        /// <summary>
        /// If the user is logged in this indicates which User Area they are logged 
        /// into; otherwise this will ne <see langword="null"/>. Typically the only 
        /// user area will be Cofoundry Admin, but some sites may have additional 
        /// custom user areas e.g. a members area.
        /// </summary>
        IUserAreaDefinition UserArea { get; set; }

        /// <summary>
        /// Indicates if the user should be required to change thier password when they log on.
        /// </summary>
        bool IsPasswordChangeRequired { get; set; }

        /// <summary>
        /// The role that this user belongs to. If this is null then the anonymous role 
        /// should be used.
        /// </summary>
        int? RoleId { get; set; }

        /// <summary>
        /// If the user belongs to a code-first role, then this will be the string identifier
        /// for that role. Otherwise this will be <see langword="null"/>.
        /// </summary>
        string RoleCode { get; set; }

        /// <summary>
        /// Returns true if the user belongs to the Cofoundry user area.
        /// </summary>
        bool IsCofoundryUser();
    }
}
