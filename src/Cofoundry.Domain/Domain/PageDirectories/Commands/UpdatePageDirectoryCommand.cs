using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the properties of an existing page directory, including
    /// properties to configure the directory path an hierarchy.
    /// </summary>
    public class UpdatePageDirectoryCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The id of the parent directory in the site hierarchy. To put this directory at the
        /// topmost level, this id should be the root directory id.
        /// </summary>
        [Required]
        [PositiveInteger]
        [Display(Name = "Parent directory", Description = "Choose the parent directory.")]
        public int ParentPageDirectoryId { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        [Required]
        [StringLength(64)]
        [Display(Name = "Name", Description = "E.g. 'About the team' or 'Our products'")]
        public string Name { get; set; }

        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
        [Required]
        [StringLength(64)]
        [Display(Name = "Url path", Description = "e.g. 'about-the-team' or 'products'.")]
        [Slug]
        public string UrlPath { get; set; }
    }
}
