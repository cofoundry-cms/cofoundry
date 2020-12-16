using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates page properties that aren't specific to a
    /// version.
    /// </summary>
    public class UpdatePageCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the page to update.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
        public ICollection<string> Tags { get; set; }
    }
}
