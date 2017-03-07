using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A very minimal representation of a user. Users are partitioned by
    /// user area so a user might be a Cofoundry admin user or could belong
    /// to a custom user area. Users cannot belong to more than one user area.
    /// </summary>
    public class UserMicroSummary
    {
        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }

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
        /// The first name is required.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is required.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Each user must be assigned to a user area (but not more than
        /// one).
        /// </summary>
        public UserAreaMicroSummary UserArea { get; set; }

        /// <summary>
        /// Joins the FirstName and LastName properties to make
        /// a full name.
        /// </summary>
        public string GetFullName()
        {
            return (FirstName + " " + LastName).Trim();
        }
    }
}
