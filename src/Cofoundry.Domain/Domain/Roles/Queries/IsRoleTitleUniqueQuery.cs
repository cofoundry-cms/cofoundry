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
    /// Determines if a role title is unique within a specific UserArea.
    /// Role titles only have to be unique per UserArea.
    /// </summary>
    public class IsRoleTitleUniqueQuery : IQuery<bool>
    {
        /// <summary>
        /// Optional database id of an existing role to exclude from the uniqueness 
        /// check. Use this when checking the uniqueness of an existing Role.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// Role titles only have to be unique per UserArea.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The role title to check for uniqueness (not case sensitive).
        /// </summary>
        public string Title { get; set; }
    }
}
