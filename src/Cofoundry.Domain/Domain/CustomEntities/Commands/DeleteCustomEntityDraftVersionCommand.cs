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
    public class DeleteCustomEntityDraftVersionCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
