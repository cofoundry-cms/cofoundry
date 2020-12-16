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
    /// Removes a page directory from the system. The root directory cannot
    /// be deleted.
    /// </summary>
    public class DeletePageDirectoryCommand : ICommand, ILoggableCommand
    {
        public DeletePageDirectoryCommand() { }

        /// <summary>
        /// Initialized the command.
        /// </summary>
        /// <param name="pageDirectoryId">Database id of the page directory to delete</param>
        public DeletePageDirectoryCommand(int pageDirectoryId)
        {
            PageDirectoryId = pageDirectoryId;
        }

        /// <summary>
        /// Database id of the page directory to delete
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }
    }
}
