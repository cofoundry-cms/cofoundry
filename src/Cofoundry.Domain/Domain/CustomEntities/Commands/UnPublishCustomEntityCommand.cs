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
    /// Sets the status of a custom entity to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    public class UnPublishCustomEntityCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The id of the custom entity to set un-published.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
