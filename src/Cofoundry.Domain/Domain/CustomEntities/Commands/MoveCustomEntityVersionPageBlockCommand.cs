using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class MoveCustomEntityVersionPageBlockCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityVersionPageBlockId { get; set; }

        public OrderedItemMoveDirection Direction { get; set; }
    }
}
