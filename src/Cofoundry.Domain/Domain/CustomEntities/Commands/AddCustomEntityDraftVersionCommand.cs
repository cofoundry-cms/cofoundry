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
    /// Creates a new draft version of a custom entity from the currently published version. If there
    /// isn't a currently published version then an exception will be thrown. An exception is also 
    /// thrown if there is already a draft version.
    /// </summary>
    public class AddCustomEntityDraftVersionCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the custom entity to add the draft version to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Optional id of a custom entity version to copy data
        /// from. If not specified then data will be copied
        /// from the latest version.
        /// </summary>
        [PositiveInteger]
        public int? CopyFromCustomEntityVersionId { get; set; }

        #region Output

        /// <summary>
        /// The database id of the newly created custom entity version. This 
        /// is set after the command has been run.
        /// </summary>
        [OutputValue]
        public int OutputCustomEntityVersionId { get; set; }

        #endregion
    }
}
