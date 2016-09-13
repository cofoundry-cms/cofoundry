using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class DeleteRoleCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }
    }
}
