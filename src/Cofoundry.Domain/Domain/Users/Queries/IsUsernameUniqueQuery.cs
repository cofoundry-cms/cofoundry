using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if a username is unique within a specific UserArea.
    /// Usernames only have to be unique per UserArea.
    /// </summary>
    public class IsUsernameUniqueQuery : IQuery<bool>
    {
        /// <summary>
        /// Optional database id of an existing user to exclude from the uniqueness 
        /// check. Use this when checking the uniqueness of an existing user.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// The username to check for uniqueness (not case sensitive).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
