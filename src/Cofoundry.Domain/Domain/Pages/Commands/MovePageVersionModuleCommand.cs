using System;
using System.Collections.Generic;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class MovePageVersionModuleCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageVersionModuleId { get; set; }

        public OrderedItemMoveDirection Direction { get; set; }
    }
}
