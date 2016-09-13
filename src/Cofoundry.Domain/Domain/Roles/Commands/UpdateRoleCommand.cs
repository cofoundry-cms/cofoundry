using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdateRoleCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        public PermissionCommandData[] Permissions { get; set; }
    }
}
