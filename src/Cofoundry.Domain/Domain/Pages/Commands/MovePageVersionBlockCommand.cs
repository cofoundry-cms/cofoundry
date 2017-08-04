using System;
using System.Collections.Generic;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class MovePageVersionBlockCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageVersionBlockId { get; set; }

        public OrderedItemMoveDirection Direction { get; set; }
    }
}
