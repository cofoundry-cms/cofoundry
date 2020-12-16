using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a user in the Cofoundry custom identity system. Users can be partitioned into
    /// different 'User Areas', which enables the identity system used by the Cofoundry administration area 
    /// to be reused for other purposes, but this isn't a common scenario and often there will only be the 
    /// Cofoundry UserArea.
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The first name is not required.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is not required.
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
        /// The users hashed password value.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Cofoundry supports upgradable password hashing and this integer value
        /// maps to the PasswordHashVersion enum to tell us which hash function
        /// type to use. An integer outside of the PasswordHashVersion enum range
        /// can be used to set up a completed custom hash provider.
        /// </summary>
        public int? PasswordHashVersion { get; set; }

        /// <summary>
        /// Used for soft deletes so we can maintain old relations and
        /// archive data.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The date the password was last changed or the that the password
        /// was first set (account create date)
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        /// <summary>
        /// The date the user last logged in, if ever.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// The date the user logged in before the LastLoginDate.
        /// </summary>
        /// <remarks>
        /// We can consider dropping this now we have a LoginLog table, I don't think it's
        /// used for anything else
        /// </remarks>
        public DateTime? PreviousLoginDate { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// The role that this user is assigned to. The role is required and
        /// determines the permissions available to the user
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The role that this user is assigned to. The role is required and
        /// determines the permissions availabel to the user
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// The Cofoundry user system can be partitioned into user areas. Each
        /// user area has a 3 letter code, e.g. The Cofoundry admin area code is
        /// 'COF'.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The Cofoundry user system can be partitioned into user areas. This enabled
        /// reuse of User functionality to create custom login areas in your application.
        /// </summary>
        public virtual UserArea UserArea { get; set; }

        /// <summary>
        /// There can be only one (system account). This is an account that
        /// can be impersonated and used to import data or in special
        /// cases used to elevate privileges
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// A flag to indicate if the users email address has been confirmed via a 
        /// sign-up notification.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        #region ICreateAuditable

        /// <summary>
        /// The date the user was created
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The UserId of the user that created this user. Nullable
        /// to allow the first user to be created.
        /// </summary>
        public int? CreatorId { get; set; }

        /// <summary>
        /// The user that created this user. Nullable
        /// to allow the first user to be created.
        /// </summary>
        public virtual User Creator { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Simply joins the first and last name together e.g. "Scott Pilgrim"
        /// </summary>
        public string GetFullName()
        {
            return (FirstName + " " + LastName).Trim();
        }

        #endregion
    }
}
