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
    /// <summary>
    /// Deletes the draft verison of a page permanently if 
    /// it exists. If no draft exists then no action is taken.
    /// </summary>
    public class DeletePageDraftVersionCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the page to delete the draft version for.
        /// </summary>
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
