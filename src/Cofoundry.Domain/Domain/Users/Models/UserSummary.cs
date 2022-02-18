using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Building on the <see cref="UserMicroSummary"/>, the UserSummary projection 
    /// contains additional audit and role data. Users are partitioned by user area 
    /// so a user might be a Cofoundry admin user or could belong to a custom user 
    /// area. Users cannot belong to more than one user area.
    /// </summary>
    public class UserSummary : UserMicroSummary, ICreateAudited
    {
        /// <summary>
        /// The first name is optional.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is optional.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email address isn't always required depending on the 
        /// user area settings.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The username is always required and depending on the user area
        /// settings this might just be a copy of the email address.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Each user must be assigned to a role which provides
        /// information about the actions a user is permitted to 
        /// perform.
        /// </summary>
        public RoleMicroSummary Role { get; set; }

        /// <summary>
        /// The date the user last signed into the application. May be
        /// <see langword="null"/> if the user has not signed in yet.
        /// </summary>
        public DateTime? LastSignInDate { get; set; }

        /// <summary>
        /// Data detailing who created the user and when.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
