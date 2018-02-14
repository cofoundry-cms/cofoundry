using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates an existing role. Also updates the role permission set.
    /// </summary>
    public class UpdateRoleCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The database id of the role to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

        /// <summary>
        /// A user friendly title for the role. Role titles must be unique 
        /// per user area and up to 50 characters.
        /// </summary>
        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The permissions set that the role should be updated to match. 
        /// You must include this otherwise all permissions will be 
        /// removed (unless of course you intend to remove all permissions).
        /// </summary>
        public ICollection<PermissionCommandData> Permissions { get; set; }
    }
}
