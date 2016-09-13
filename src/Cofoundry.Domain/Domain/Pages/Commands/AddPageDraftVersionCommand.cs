using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Creates a new draft version of a page from the currently published version. If there
    /// isn't a currently published version then an exception will be thrown. An exception is also 
    /// thrown if there is already a draft version.
    /// </summary>
    public class AddPageDraftVersionCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageId { get; set; }

        [PositiveInteger]
        public int? CopyFromPageVersionId { get; set; }

        #region Output

        [OutputValue]
        public int OutputPageVersionId { get; set; }

        #endregion
    }
}
