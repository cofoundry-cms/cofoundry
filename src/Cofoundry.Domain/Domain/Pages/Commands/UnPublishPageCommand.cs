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
    /// Sets the status of a page to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    public class UnPublishPageCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The id of the page to set un-published.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }
    }
}
