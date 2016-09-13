using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? PasswordEncryptionVersion { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        public DateTime? PreviousLoginDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool RequirePasswordChange { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public string UserAreaCode { get; set; }
        public virtual UserArea UserArea { get; set; }

        #region ICreateAuditable

        public DateTime CreateDate { get; set; }
        public int? CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion

        #region methods

        public string GetFullName()
        {
            return (FirstName + " " + LastName).Trim();
        }

        #endregion
    }
}
