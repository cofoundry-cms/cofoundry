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
        /// <summary>
        /// Id of the page to add the draft version to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageId { get; set; }

        /// <summary>
        /// Optional id of a page version to copy data
        /// from. If not specified then data will be copied
        /// from the latest version.
        /// </summary>
        [PositiveInteger]
        public int? CopyFromPageVersionId { get; set; }

        #region Output

        /// <summary>
        /// The database id of the newly created page version. This 
        /// is set after the command has been run.
        /// </summary>
        [OutputValue]
        public int OutputPageVersionId { get; set; }

        #endregion
    }
}
