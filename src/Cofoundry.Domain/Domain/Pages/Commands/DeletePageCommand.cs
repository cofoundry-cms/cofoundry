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
    /// Deletes a page and all associated versions permanently.
    /// </summary>
    public class DeletePageCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the page to delete.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }
    }
}
