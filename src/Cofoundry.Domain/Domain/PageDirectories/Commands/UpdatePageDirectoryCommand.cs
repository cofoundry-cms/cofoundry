using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the main properties of an existing page directory. To
    /// update properties that affect the route, use <see cref="UpdatePageDirectoryUrlCommand"/>.
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
        /// User friendly display name of the directory.
        /// </summary>
        [Required]
        [StringLength(64)]
        [Display(Name = "Name", Description = "E.g. 'About the team' or 'Our products'")]
        public string Name { get; set; }
    }
}
