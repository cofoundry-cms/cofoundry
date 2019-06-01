using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Deletes a block from a template region on a custom entity page.
    /// </summary>
    public class DeleteCustomEntityVersionPageBlockCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the block to delete.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityVersionPageBlockId { get; set; }
    }
}
