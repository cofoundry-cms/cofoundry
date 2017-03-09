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
    /// Deletes a web directory with the specified database id.
    /// </summary>
    public class DeleteWebDirectoryCommand : ICommand, ILoggableCommand
    {
        public DeleteWebDirectoryCommand() { }

        /// <summary>
        /// Initialized the command.
        /// </summary>
        /// <param name="webDirectoryId">Database id of the web directory to delete</param>
        public DeleteWebDirectoryCommand(int webDirectoryId)
        {
            WebDirectoryId = webDirectoryId;
        }

        /// <summary>
        /// Database id of the web directory to delete
        /// </summary>
        [Required]
        [PositiveInteger]
        public int WebDirectoryId { get; set; }
    }
}
