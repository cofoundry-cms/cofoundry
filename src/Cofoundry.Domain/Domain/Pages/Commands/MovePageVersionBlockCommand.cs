using System;
using System.Collections.Generic;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Moves a block up or down within a multi-block region 
    /// on a page.
    /// </summary>
    public class MovePageVersionBlockCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the block to mvoe.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// The direction to move the block within the collection.
        /// </summary>
        public OrderedItemMoveDirection Direction { get; set; }
    }
}
