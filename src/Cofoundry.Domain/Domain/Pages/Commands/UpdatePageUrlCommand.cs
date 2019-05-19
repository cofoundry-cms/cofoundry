using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the url of a page.
    /// </summary>
    public class UpdatePageUrlCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the page to update.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        /// <summary>
        /// The directory to put the page in.
        /// </summary>
        [Required(ErrorMessage = "Please choose a directory")]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Optional locale id of the page if used in a localized site.
        /// </summary>
        [PositiveInteger]
        public int? LocaleId { get; set; }

        /// <summary>
        /// The path of the page within the directory. This must be
        /// unique within the directory the page is parented to.
        /// E.g. 'about-the-team'
        /// </summary>
        [StringLength(70)]
        [Slug]
        public virtual string UrlPath { get; set; }

        /// <summary>
        /// If this is a CustomEntityDetails page, this will need to be set
        /// to a value that matches the RouteFormat of an existing
        /// ICustomEntityRoutingRule e.g. "{Id}/{UrlSlug}".
        /// </summary>
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
