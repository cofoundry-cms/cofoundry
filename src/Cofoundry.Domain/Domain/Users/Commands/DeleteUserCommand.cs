using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marks a user as deleted in the database (soft delete).
    /// </summary>
    public class DeleteUserCommand : ICommand, ILoggableCommand
    {
        public DeleteUserCommand() { }

        /// <summary>
        /// Initializes the command with parameters.
        /// </summary>
        /// <param name="userId">Id of the user to delete.</param>
        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Id of the user to delete.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }
    }
}
