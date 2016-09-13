using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddRoleCommand : ICommand, ILoggableCommand
    {
        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        public string UserAreaCode { get; set; }

        public PermissionCommandData[] Permissions { get; set; }

        #region Output

        [OutputValue]
        public int OutputRoleId { get; set; }

        #endregion
    }
}
