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
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        [PositiveInteger]
        public int? CopyFromCustomEntityVersionId { get; set; }

        #region Output

        [OutputValue]
        public int OutputCustomEntityVersionId { get; set; }

        #endregion
    }
}
