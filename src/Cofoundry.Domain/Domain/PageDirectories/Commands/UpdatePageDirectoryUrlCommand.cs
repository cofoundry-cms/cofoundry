using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the url of a page directory. Changing a directory url
    /// will cause the url of any child directories or pages to change. The command
    /// will publish an <see cref="PageDirectoryUrlChangedMessage"/> or <see cref="PageUrlChangedMessage"/>
    /// for any affected directories or pages.
    /// </summary>
    public class UpdatePageDirectoryUrlCommand : ICommand, ILoggableCommand
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
        [NotEqual(nameof(PageDirectoryId), ErrorMessage = "The directory cannot be parented to itself.")]
        public int ParentPageDirectoryId { get; set; }

        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Url path", Description = "e.g. 'about-the-team' or 'products'.")]
        [Slug]
        public string UrlPath { get; set; }
    }
}
