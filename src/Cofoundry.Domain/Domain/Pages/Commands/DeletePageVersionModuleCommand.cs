using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class DeletePageVersionModuleCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int PageVersionModuleId { get; set; }
    }
}
