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
    public class DeletePageDraftVersionCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions()
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
