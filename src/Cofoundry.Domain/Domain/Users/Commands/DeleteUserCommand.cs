using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class DeleteUserCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }
    }
}
