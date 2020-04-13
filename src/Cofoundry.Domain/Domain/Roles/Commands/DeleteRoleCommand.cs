using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Deletes a role with the specified database id. Roles cannot be
    /// deleted if assigned to users.
    /// </summary>
    public class DeleteRoleCommand : ICommand, ILoggableCommand
    {
        public DeleteRoleCommand() { }

        /// <summary>
        /// Initialized the command with the specified roleId
        /// </summary>
        /// <param name="roleId">Id of the role to delete.</param>
        public DeleteRoleCommand(int roleId)
        {
            RoleId = roleId;
        }

        /// <summary>
        /// Id of the role to delete.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }
    }
}
