using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdatePageUrlCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        [Required(ErrorMessage = "Please choose a web directory")]
        [PositiveInteger]
        public int WebDirectoryId { get; set; }

        [PositiveInteger]
        public int? LocaleId { get; set; }

        [StringLength(70)]
        [Slug]
        public virtual string UrlPath { get; set; }

        [StringLength(70)]
        public string CustomEntityRoutingRule { get; set; }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions()
        {
            yield return new PageUpdateUrlPermission();
        }

        #endregion
    }
}
